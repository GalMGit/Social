
import { useEffect, useState, type FormEvent } from "react";
import { useNavigate } from "react-router-dom";


import "./PostCreate.css";
import {mediaService} from "../../../services/media-service/mediaService.ts";
import type {CreatePostRequest} from "../../../models/posts/requests/CreatePostRequest.ts";
import {postService} from "../../../services/post-service/postService.ts";

export default function PostCreate() {
    const navigate = useNavigate();

    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [communityId, setCommunityId] = useState("");

    const [image, setImage] = useState<File | null>(null);
    const [preview, setPreview] = useState<string | null>(null);

    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!image) {
            setPreview(null);
            return;
        }

        const objectUrl = URL.createObjectURL(image);
        setPreview(objectUrl);

        return () => URL.revokeObjectURL(objectUrl);
    }, [image]);

    const handleImageChange = (
        event: React.ChangeEvent<HTMLInputElement>
    ) => {
        const file = event.target.files?.[0] ?? null;

        if (!file) {
            setImage(null);
            return;
        }

        if (!file.type.startsWith("image/")) {
            setError("Выберите файл изображения.");
            event.target.value = "";
            setImage(null);
            return;
        }

        if (file.size > 10 * 1024 * 1024) {
            setError("Размер изображения не должен превышать 10 МБ.");
            event.target.value = "";
            setImage(null);
            return;
        }

        setError(null);
        setImage(file);
    };

    const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (isSubmitting) return;

        setError(null);
        setIsSubmitting(true);

        try {
            let imagePath: string | null = null;

            if (image) {
                const uploaded = await mediaService.upload(image);
                imagePath = uploaded.imagePath;

            }

            const request: CreatePostRequest = {
                title: title.trim(),
                content: content.trim(),
                imagePath,
                communityId: communityId.trim() || null,
            };

            console.log("3. JSON для Posts:", JSON.stringify(request, null, 2));

            const postId = await postService.create(request);

            console.log("4. ID созданного поста:", postId);

            navigate(`/posts/${postId}`);
        } catch {
            setError(
                "Не удалось создать пост. Проверьте соединение и попробуйте ещё раз."
            );
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <main className="post-create">
            <header className="post-create__header">
                <h1>Создать пост</h1>
                <p>Поделитесь тем, что хотите рассказать.</p>
            </header>

            <form
                className="post-create__form"
                onSubmit={handleSubmit}
            >
                <div className="post-create__field">
                    <label htmlFor="post-title">Заголовок</label>
                    <input
                        id="post-title"
                        type="text"
                        value={title}
                        onChange={(event) => setTitle(event.target.value)}
                        placeholder="О чём ваш пост?"
                        maxLength={200}
                        required
                    />
                </div>

                <div className="post-create__field">
                    <label htmlFor="post-content">Содержание</label>
                    <textarea
                        id="post-content"
                        value={content}
                        onChange={(event) => setContent(event.target.value)}
                        placeholder="Напишите что-нибудь..."
                        rows={7}
                        required
                    />
                </div>

                <div className="post-create__field">
                    <label htmlFor="post-image">Изображение</label>
                    <input
                        id="post-image"
                        type="file"
                        accept="image/*"
                        onChange={handleImageChange}
                    />

                    {preview && (
                        <div className="post-create__preview">
                            <img src={preview} alt="Предпросмотр изображения" />

                            <button
                                type="button"
                                className="post-create__remove-image"
                                onClick={() => setImage(null)}
                            >
                                Удалить изображение
                            </button>
                        </div>
                    )}
                </div>

                <div className="post-create__field">
                    <label htmlFor="post-community">
                        Community ID (необязательно)
                    </label>
                    <input
                        id="post-community"
                        type="text"
                        value={communityId}
                        onChange={(event) =>
                            setCommunityId(event.target.value)
                        }
                        placeholder="UUID сообщества"
                    />
                </div>

                {error && (
                    <p className="post-create__error" role="alert">
                        {error}
                    </p>
                )}

                <div className="post-create__actions">
                    <button
                        type="button"
                        className="post-create__cancel"
                        disabled={isSubmitting}
                        onClick={() => navigate(-1)}
                    >
                        Отмена
                    </button>

                    <button
                        type="submit"
                        className="post-create__submit"
                        disabled={
                            isSubmitting ||
                            !title.trim() ||
                            !content.trim()
                        }
                    >
                        {isSubmitting ? "Создание..." : "Создать пост"}
                    </button>
                </div>
            </form>
        </main>
    );
}
