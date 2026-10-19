# Social Network

Микросервисная backend-платформа социальной сети на **.NET 10**, ASP.NET Core, PostgreSQL, RabbitMQ, Redis и YARP. HTTP-запросы проходят через API Gateway, а межсервисные события передаются асинхронно через Wolverine и RabbitMQ.

> Документ описывает архитектуру и настройки, обнаруженные в репозитории и предоставленной конфигурации Docker Compose. Конкретные DTO, схемы ответов и полный список endpoint-ов следует сверять с OpenAPI.

## Содержание

- [Возможности](#возможности)
- [Архитектура](#архитектура)
- [Технологический стек](#технологический-стек)
- [Структура проекта](#структура-проекта)
- [Сервисы](#сервисы)
- [Взаимодействие и события](#взаимодействие-и-события)
- [Запуск через Docker Compose](#запуск-через-docker-compose)
- [Переменные окружения](#переменные-окружения)
- [Сеть и публикация портов](#сеть-и-публикация-портов)
- [API и OpenAPI](#api-и-openapi)
- [Аутентификация и ограничения запросов](#аутентификация-и-ограничения-запросов)
- [Базы данных и миграции](#базы-данных-и-миграции)
- [Локальная разработка](#локальная-разработка)
- [Диагностика](#диагностика)
- [Рекомендации для production](#рекомендации-для-production)

## Возможности

- Регистрация, вход и подтверждение email.
- Пользовательские профили и аватары.
- Создание, получение и удаление публикаций.
- Создание и просмотр сообществ.
- Создание и чтение комментариев.
- Загрузка медиа в S3-совместимое объектное хранилище.
- Асинхронная обработка событий и отправка email.
- Единая HTTP-точка входа через YARP Gateway.
- JWT Bearer-аутентификация, политики доступа и ограничение частоты запросов.

Фактические операции, HTTP-методы и контракты ответов определяются endpoint-ами сервисов и маршрутами Gateway.

## Архитектура

```text
                           HTTP + Bearer JWT
                                  |
                                  v
                         +------------------+
                         |  social.gateway  |
                         |   YARP / Scalar  |
                         +------------------+
                           |   |   |   |   \
                           |   |   |   |    \
                           v   v   v   v     v
                       Identity Users Posts Communities Comments Media
                           |      |    |       |       |       |
                           +------+----+-------+-------+-------+
                                  |       Docker network
                      +-----------+-----------+-----------+
                      |                       |           |
                      v                       v           v
                 PostgreSQL               RabbitMQ      Redis
                      |
               service-specific DBs

                 RabbitMQ ---> EmailWorker
                 Media service ---> S3-compatible storage
```

Основные принципы:

1. **API Gateway.** Клиент обращается к Gateway, а YARP направляет запрос к нужному микросервису.
2. **Внутренняя сеть.** Сервисы взаимодействуют по DNS-именам Docker Compose, например `http://social.posts:8080/`. Внутренние порты не требуется публиковать на хост.
3. **Разделение данных.** Сервисы используют отдельные имена баз данных и собственные контексты доступа к данным в общем экземпляре PostgreSQL.
4. **Асинхронная интеграция.** События передаются через RabbitMQ; Wolverine обеспечивает обработку сообщений и интеграцию с PostgreSQL.
5. **Внешнее хранилище медиа.** Файлы размещаются в S3-совместимом хранилище, а доступ к нему задаётся конфигурацией.
6. **Отдельный worker.** EmailWorker — фоновый процесс, не требующий HTTP-порта для внешних клиентов.

## Технологический стек

| Технология | Назначение |
|---|---|
| .NET 10 / ASP.NET Core | HTTP API и фоновые приложения |
| YARP | Reverse proxy и маршрутизация |
| Scalar / OpenAPI | Интерактивная API-документация |
| PostgreSQL | Реляционные данные и persistence сообщений Wolverine |
| Entity Framework Core | ORM и миграции |
| RabbitMQ | Асинхронная доставка событий |
| Wolverine | Messaging, durable inbox/outbox и обработчики |
| Redis | Кэш/временные данные, используемые отдельными сервисами |
| S3-compatible storage | Хранение медиафайлов |
| MailKit / SMTP | Отправка электронной почты |
| Docker Compose | Запуск контейнеров и зависимостей |

Версии NuGet-пакетов управляются централизованно через `Directory.Packages.props`, общие настройки сборки — через `Directory.Build.props`.

## Структура проекта

```text
src/
├── Gateway/
│   └── Social.Gateway/
│       ├── Program.cs
│       └── yarp.*.json
├── Services/
│   ├── Identity/Social.Identity/
│   ├── Users/Social.Users/
│   ├── Posts/Social.Posts/
│   ├── Communities/Social.Communities/
│   ├── Comments/Social.Comments/
│   ├── Media/Social.Media/
│   └── Email/Social.EmailWorker/
└── BuildingBlocks/
    ├── Social.Contracts/
    │   └── Events/
    └── Social.Shared/

Directory.Build.props
Directory.Packages.props
.dockerignore
.gitignore
```

- `Services` — бизнес-сервисы и их API/обработчики.
- `BuildingBlocks/Social.Contracts` — общие контракты межсервисных событий.
- `BuildingBlocks/Social.Shared` — общая инфраструктура, включая настройки аутентификации, endpoint-ов и OpenAPI.
- `Gateway/Social.Gateway/yarp.*.json` — конфигурации маршрутов и кластеров YARP.

## Сервисы

| Сервис | Ответственность |
|---|---|
| `social.gateway` | Единая точка HTTP-входа, маршрутизация YARP, политики доступа, rate limiting и Scalar UI |
| `social.identity` | Регистрация, вход, подтверждение email, JWT и начальная настройка администратора |
| `social.users` | Профили пользователей и операции с аватаром |
| `social.posts` | Публикации и связанные события |
| `social.communities` | Сообщества и связанные события |
| `social.comments` | Комментарии и обработка событий публикаций/пользователей |
| `social.media` | Загрузка и управление медиа, обработка события удаления публикации |
| `social.emailworker` | Получение сообщений из очереди `social-email` и отправка email через SMTP |
| `postgres` | PostgreSQL |
| `rabbitmq` | Брокер сообщений RabbitMQ |
| `redis` | Redis |

## Взаимодействие и события

### HTTP через YARP

В конфигурации Gateway сервисы адресуются по именам Docker Compose:

| Сервис | Внутренний адрес |
|---|---|
| Identity | `http://social.identity:8080/` |
| Users | `http://social.users:8080/` |
| Posts | `http://social.posts:8080/` |
| Communities | `http://social.communities:8080/` |
| Comments | `http://social.comments:8080/` |
| Media | `http://social.media:8080/` |

Не заменяйте эти адреса на `localhost`: внутри контейнера `localhost` указывает на сам контейнер, а не на соседний сервис.

### Контракты событий

Общие типы находятся в `src/BuildingBlocks/Social.Contracts/Events`. В проекте присутствуют, в частности:

- `UserStartRegistrationEvent` — начало регистрации и данные, необходимые для подтверждения email.
- `UserProfileCreatedEvent` — создан профиль пользователя.
- `PostCreatedEvent` — создана публикация.
- `PostDeletedEvent` — публикация удалена; контракт включает ключи связанных медиаобъектов.
- `CommunityCreatedEvent` и `CommunityDeletedEvent` — события жизненного цикла сообщества.

### RabbitMQ

По текущей конфигурации используются следующие exchanges и очереди:

| Exchange / очередь | Назначение в конфигурации |
|---|---|
| `posts-events` | Fanout exchange с подписчиками `social-comments` и `social-media` |
| `users-events` | Fanout exchange с подписчиками `social-posts` и `social-comments` |
| `communities-events` | Fanout exchange с очередью `social-posts` |
| `social-email` | Очередь фонового EmailWorker |
| `social-posts` | Очередь обработчиков Posts |
| `social-communities` | Очередь обработчиков Communities |
| `social-comments` | Очередь обработчиков Comments |
| `social-media` | Очередь обработчиков Media |

Топология может изменяться по мере развития приложения. При изменении контракта события обновляйте отправителя, получателей и общий контракт согласованно.

В нескольких сервисах настроены Wolverine durable outbox/inbox и хранение сообщений в PostgreSQL. Это повышает устойчивость доставки, но не отменяет возможных повторных доставок: обработчики должны быть рассчитаны на дубликаты и идемпотентность.

## Запуск через Docker Compose

### Требования

- Docker Engine.
- Docker Compose v2 (`docker compose`).
- Доступ к NuGet для сборки образов.
- SMTP-учётные данные для отправки email.
- S3-совместимое хранилище и параметры доступа для работы с медиа.
- Свободный порт для Gateway.

Проверьте установку:

```bash
docker --version
docker compose version
```

### 1. Создайте `.env`

Создайте файл `.env` рядом с Compose-файлом и заполните переменные из раздела [Переменные окружения](#переменные-окружения). Реальные пароли и ключи не коммитьте в Git. Файл `.env` исключён из Git через `.gitignore`.

### 2. Проверьте Compose-конфигурацию

```bash
docker compose config
```

Команда проверяет YAML и подстановку переменных окружения. Осторожно: итоговый вывод может содержать секреты — не публикуйте его целиком.

### 3. Соберите и запустите систему

```bash
docker compose up -d --build
```

Если Compose-файл имеет другое имя:

```bash
docker compose -f <compose-file> up -d --build
```

### 4. Проверьте состояние

```bash
docker compose ps
docker compose logs -f social.gateway
```

Логи отдельного сервиса:

```bash
docker compose logs -f social.identity
docker compose logs -f social.posts
docker compose logs -f social.communities
docker compose logs -f social.emailworker
```

### 5. Остановка

```bash
docker compose down
```

Команда `docker compose down -v` дополнительно удаляет именованные volumes и постоянные данные PostgreSQL, RabbitMQ и Redis. Используйте её только если намеренно хотите удалить эти данные.

## Переменные окружения

Имена ниже соответствуют показанной конфигурации Compose. Точные допустимые форматы некоторых значений определяются соответствующими классами настроек приложения.

### PostgreSQL

| Переменная | Назначение |
|---|---|
| `POSTGRES_USER` | Пользователь PostgreSQL |
| `POSTGRES_PASSWORD` | Пароль PostgreSQL |
| `IDENTITY_DB` | Имя базы Identity |
| `USERS_DB` | Имя базы Users |
| `POSTS_DB` | Имя базы Posts |
| `COMMUNITIES_DB` | Имя базы Communities |
| `COMMENTS_DB` | Имя базы Comments |

Сервисы подключаются к `postgres:5432`. В показанном Compose не задан `POSTGRES_DB` и не подключён init-скрипт создания всех перечисленных баз. Bootstrap Identity создаёт свою базу, однако для остальных баз необходимо обеспечить создание до миграций/инициализации сервисов. `pg_isready` подтверждает доступность сервера PostgreSQL, но не существование каждой прикладной базы.

### RabbitMQ

| Переменная | Назначение |
|---|---|
| `RABBITMQ_USER` | Пользователь RabbitMQ |
| `RABBITMQ_PASSWORD` | Пароль RabbitMQ |
| `RABBITMQ_PORT` | Внутренний AMQP-порт, обычно `5672` |

### Redis

| Переменная | Назначение |
|---|---|
| `REDIS_PASSWORD` | Пароль Redis |

Сервисы подключаются к Redis по внутреннему адресу `redis:6379`.

### ASP.NET Core и JWT

| Переменная | Назначение |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | Окружение, например `Development` или `Production` |
| `JWT_SECRET_KEY` | Секрет подписи JWT |
| `JWT_EXPIRES` | Время действия токена в формате, ожидаемом `JwtOptions` |

### Администратор Identity

| Переменная | Назначение |
|---|---|
| `ADMIN_USERNAME` | Имя администратора |
| `ADMIN_EMAIL` | Email администратора |
| `ADMIN_PASSWORD` | Пароль администратора |

### SMTP

| Переменная | Назначение |
|---|---|
| `SMTP_SERVER` | SMTP-сервер |
| `SMTP_PORT` | SMTP-порт |
| `SMTP_USERNAME` | Пользователь SMTP |
| `SMTP_PASSWORD` | Пароль SMTP |
| `SMTP_SENDER_EMAIL` | Адрес отправителя |
| `SMTP_SENDER_NAME` | Отображаемое имя отправителя |

### S3-совместимое хранилище

| Переменная | Назначение |
|---|---|
| `PUBLIC_STORAGE_OPTIONS__SERVICE_URL` | Endpoint хранилища |
| `PUBLIC_STORAGE_OPTIONS__BUCKET` | Имя bucket |
| `PUBLIC_STORAGE_OPTIONS__ACCESS_KEY` | Access key |
| `PUBLIC_STORAGE_OPTIONS__SECRET_KEY` | Secret key |
| `PUBLIC_STORAGE_OPTIONS__BASE_URL` | Базовый URL объектов |

В конфигурации .NET двойное подчёркивание `__` используется для разделения уровней вложенных ключей. Не храните реальные ключи доступа к объектному хранилищу в README, коммитах или issue.

### Порты

| Переменная | Назначение |
|---|---|
| `GATEWAY_PORT` | Порт Gateway, опубликованный на хост |

После отключения публикации портов микросервисов переменные `IDENTITY_PORT`, `USERS_PORT`, `POSTS_PORT`, `COMMUNITIES_PORT`, `COMMENTS_PORT` и `MEDIA_PORT` не нужны для этой Compose-конфигурации.

## Сеть и публикация портов

Рекомендуемый вариант — публиковать только Gateway.

| Контейнер | Порт внутри Docker | Публикация на хост |
|---|---:|---|
| `social.gateway` | `8080` | Да: `${GATEWAY_PORT}:8080` |
| HTTP-микросервисы | `8080` | Нет |
| PostgreSQL | `5432` | Нет |
| RabbitMQ AMQP | `5672` | Нет |
| RabbitMQ Management UI | `15672` | Нет |
| Redis | `6379` | Нет |
| EmailWorker | — | Не требуется |

Для связи между контейнерами `ports` не нужны. Compose подключает сервисы к общей сети проекта и предоставляет DNS по имени сервиса. `expose` необязателен и сам по себе не является средством защиты.

Проверьте публикацию:

```bash
docker compose ps
```

Только у Gateway должно быть отображение host-port → `8080`. У внутренних сервисов не должно быть привязок вида `0.0.0.0:5432->5432/tcp` или `0.0.0.0:<port>->...`.

Если разработчику временно нужен доступ к инфраструктуре с хост-машины, публикуйте порт только на loopback, например `127.0.0.1:5432:5432`. Не открывайте PostgreSQL, RabbitMQ и Redis в интернет.

## API и OpenAPI

Gateway агрегирует OpenAPI-документы микросервисов и предоставляет Scalar UI.

После запуска:

- Scalar UI: `http://localhost:<GATEWAY_PORT>/docs`
- Identity OpenAPI: `/openapi/identity/v1.json`
- Posts OpenAPI: `/openapi/posts/v1.json`
- Users OpenAPI: `/openapi/users/v1.json`
- Comments OpenAPI: `/openapi/comments/v1.json`
- Media OpenAPI: `/openapi/media/v1.json`
- Communities OpenAPI: `/openapi/communities/v1.json`

Для защищённых маршрутов используйте Bearer JWT, полученный через Identity.

### Маршруты, объявленные в YARP

| Метод | Путь | Назначение |
|---|---|---|
| `POST` | `/api/v1/auth/register` | Регистрация |
| `POST` | `/api/v1/auth/login` | Вход |
| `POST` | `/api/v1/auth/email/confirm` | Подтверждение email |
| `POST` | `/api/v1/posts` | Создание публикации |
| `GET` | `/api/v1/posts` | Список публикаций |
| `GET` | `/api/v1/posts/{postId}` | Получение публикации |
| `DELETE` | `/api/v1/posts/{postId}` | Удаление публикации |
| `POST` | `/api/v1/communities` | Создание сообщества |
| `GET` | `/api/v1/communities` | Список сообществ |
| `GET` | `/api/v1/communities/{communityId}` | Получение сообщества |
| `DELETE` | `/api/v1/communities/{communityId}` | Удаление сообщества |
| `GET` | `/api/v1/users/{userId}/profile` | Получение профиля |
| `PATCH` | `/api/v1/users/me/avatar` | Обновление аватара текущего пользователя |
| `POST` | `/api/v1/comments` | Создание комментария |
| `GET` | `/api/v1/comments/post/{postId}` | Комментарии публикации |
| `GET` | `/api/v1/comments/{commentId}` | Получение комментария |
| `POST` | `/api/v1/media` | Загрузка медиа |

Полные схемы запросов и ответов смотрите в OpenAPI.

## Аутентификация и ограничения запросов

Gateway регистрирует Bearer-аутентификацию и policy `Authenticated`. Для каждого маршрута доступ задаётся в `yarp.*.json`. Некоторые GET-маршруты доступны анонимно, а операции создания обычно требуют авторизации.

Rate limiting в Gateway настроен на основе IP-адреса. При превышении лимита возвращается `429 Too Many Requests`.

| Политика | Лимит |
|---|---:|
| `auth-ip-limit` | 5 запросов в минуту |
| `posts-create-ip-limit` | 10 запросов в минуту |
| `posts-list-ip-limit` | 60 запросов в минуту |
| `post-by-id-ip-limit` | 120 запросов в минуту |
| `communities-create-ip-limit` | 10 запросов в минуту |
| `communities-list-ip-limit` | 60 запросов в минуту |
| `community-by-id-ip-limit` | 120 запросов в минуту |
| `media-create-ip-limit` | 10 запросов в минуту |
| `comment-create-ip-limit` | 20 запросов в минуту |

Лимиты локальны для экземпляра Gateway; несколько реплик не образуют единый распределённый лимитер. При ошибке проксирования Gateway пишет warning и возвращает `503 Service Unavailable`.

**Проверка перед production:** в предоставленной конфигурации `yarp.communities.json` маршрут `DELETE /api/v1/communities/{communityId}` имеет `AuthorizationPolicy: anonymous`. Если удаление должно требовать JWT, установите `Authenticated` и отдельно проверьте проверку владельца в обработчике. Скрытие внутренних портов не заменяет авторизацию бизнес-операций.

## Базы данных и миграции

Используются отдельные строки подключения:

- `IdentityDb`
- `UsersDb`
- `PostsDb`
- `CommunitiesDb`
- `CommentsDb`

Каждый микросервис использует собственную базу данных PostgreSQL, указанную в конфигурации подключения.

При запуске микросервисы автоматически выполняют необходимую инициализацию базы данных и применяют миграции Entity Framework Core. Поэтому при корректно настроенных строках подключения и доступном PostgreSQL ручное создание баз перед запуском не требуется.

Используемые базы данных задаются переменными окружения:

IDENTITY_DB

USERS_DB

POSTS_DB

COMMUNITIES_DB

COMMENTS_DB

Для работы автоматической инициализации PostgreSQL должен быть доступен, а пользователь подключения — иметь необходимые права на создание баз данных и выполнение миграций.

В production рекомендуется контролировать выполнение миграций, делать резервные копии перед изменениями схемы и учитывать совместимость миграций с работающими версиями сервисов.

## Локальная разработка

Восстановление NuGet-пакетов и сборка:

```bash
dotnet restore
dotnet build
```

Если solution-файл находится не в текущем каталоге, укажите его путь явно.

Для запуска сервиса из IDE требуются доступные PostgreSQL, RabbitMQ и Redis и корректные строки подключения. Имена `postgres`, `rabbitmq` и `redis` разрешаются внутри Docker-сети; процесс, запущенный непосредственно на хосте, не сможет использовать их как DNS-имена без дополнительной настройки. Для локальной разработки можно временно привязать нужные порты к `127.0.0.1`.

При добавлении endpoint-а:

1. Реализуйте endpoint и обработчик в соответствующем сервисе.
2. Проверьте OpenAPI-схему.
3. Добавьте маршрут и кластер в нужный `yarp.*.json`, если API должен быть доступен через Gateway.
4. Настройте подходящую authorization policy и rate-limit policy.
5. При использовании событий обновите контракт в `Social.Contracts`, producer и consumer.

## Диагностика

### Контейнер не запускается

```bash
docker compose ps
docker compose logs --tail=200 <service-name>
```

### Проверка RabbitMQ

```bash
docker compose logs --tail=200 rabbitmq
docker compose exec rabbitmq rabbitmq-diagnostics -q ping
```

Management UI на порту `15672` доступен с хоста только если порт явно опубликован. В закрытой конфигурации его не следует публиковать без необходимости.

### Gateway возвращает 503

Проверьте, что целевой контейнер запущен, слушает `8080`, находится в общей Docker-сети и имя сервиса совпадает с адресом в YARP. Изучите логи Gateway и целевого сервиса.

### Ошибка подключения к PostgreSQL

Проверьте контейнер `postgres`, учётные данные, имя базы из `*_DB` и наличие базы. `pg_isready` проверяет доступность сервера, но не существование каждой базы приложения.

### Email не отправляется

Проверьте SMTP host/port, имя пользователя, пароль, адрес отправителя и доступность SMTP из EmailWorker. Изучите логи worker-а и состояние очереди `social-email`.

### Очистка неиспользуемых Docker-образов

```bash
docker image prune
```

Чтобы удалить все образы, не используемые существующими контейнерами:

```bash
docker image prune -a
```

Вторая команда может потребовать повторной загрузки базовых образов при следующей сборке.

## Рекомендации для production

- Публикуйте только Gateway; не выставляйте наружу порты микросервисов, PostgreSQL, RabbitMQ и Redis.
- Используйте HTTPS перед Gateway и корректно настраивайте forwarded headers при наличии reverse proxy/load balancer.
- Храните секреты в secret manager или переменных окружения среды развёртывания, не в Git.
- Используйте минимально необходимые права для пользователей PostgreSQL и RabbitMQ.
- Настройте CORS под точный origin frontend-приложения. В коде Gateway сейчас указан `http://localhost:5173`.
- Проверьте authorization policy каждого маршрута, особенно операций изменения и удаления.
- Настройте health/readiness checks, мониторинг очередей RabbitMQ и наблюдаемость фоновых обработчиков.
- Планируйте резервное копирование и восстановление PostgreSQL и объектного хранилища.
- Проверьте идемпотентность обработчиков и стратегию обработки сообщений, которые постоянно завершаются ошибкой.
- Не рассматривайте внутреннюю Docker-сеть как замену аутентификации и авторизации.
