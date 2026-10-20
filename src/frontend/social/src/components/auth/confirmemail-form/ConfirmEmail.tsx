import { useState } from "react";
import axios from "axios";
import { authService } from "../../../services/auth-service/authService.ts";
import "./CofirmEmail.css";

export const ConfirmEmail = () => {
    const [email, setEmail] = useState("");
    const [code, setCode] = useState("");
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [isConfirmed, setIsConfirmed] = useState(false);

    const handleSubmit = async (
        event: React.FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        setError("");
        setIsLoading(true);

        try {
            await authService.confirmEmail({
                email,
                code,
            });

            setIsConfirmed(true);
        } catch (error: unknown) {
            if (axios.isAxiosError(error)) {
                setError(
                    error.response?.data?.message ??
                    "Не удалось подтвердить email. Попробуйте ещё раз."
                );
            } else {
                setError("Произошла непредвиденная ошибка.");
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="login-form">
            <div className="auth-form-header">
                <span className="auth-form-label">
                    CONFIRM EMAIL
                </span>

                <h1>
                    {isConfirmed
                        ? "Email подтверждён"
                        : "Подтвердите email"}
                </h1>

                <p>
                    {isConfirmed
                        ? "Ваш адрес электронной почты успешно подтверждён."
                        : "Введите email и код из письма, чтобы подтвердить свой аккаунт."}
                </p>
            </div>

            {!isConfirmed && (
                <form onSubmit={handleSubmit}>
                    <div className="form-field">
                        <label htmlFor="confirm-email">
                            Email
                        </label>

                        <input
                            id="confirm-email"
                            type="email"
                            placeholder="you@example.com"
                            value={email}
                            onChange={(event) =>
                                setEmail(event.target.value)
                            }
                            autoComplete="email"
                            required
                        />
                    </div>

                    <div className="form-field">
                        <label htmlFor="confirm-code">
                            Код подтверждения
                        </label>

                        <input
                            id="confirm-code"
                            type="text"
                            placeholder="Введите код"
                            value={code}
                            onChange={(event) =>
                                setCode(event.target.value)
                            }
                            required
                        />
                    </div>

                    {error && (
                        <div className="auth-error">
                            {error}
                        </div>
                    )}

                    <button
                        className="auth-submit-button"
                        type="submit"
                        disabled={isLoading}
                    >
                        {isLoading
                            ? "Подтверждение..."
                            : "Подтвердить email"}
                    </button>
                </form>
            )}
        </div>
    );
};