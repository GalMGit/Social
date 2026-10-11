import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./RegisterForm.css";
import {authService} from "../../../services/auth-service/authService.ts";

export const RegisterForm = () => {
    const navigate = useNavigate();

    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        setError(null);

        if (password !== confirmPassword) {
            setError("Пароли не совпадают.");
            return;
        }

        setLoading(true);

        try {
            await authService.register({
                username,
                email,
                password,
                confirmPassword,
            });

            navigate("/auth/login", {
                replace: true,
            });
        } catch (error: any) {
            const message =
                error.response?.data?.message ??
                "Не удалось создать аккаунт.";

            setError(message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="register-form">
            <div className="auth-form-header">
                <span className="auth-form-label">GET STARTED</span>

                <h1>Create account</h1>

                <p>
                    Create your account to start shopping.
                </p>
            </div>

            <form onSubmit={handleSubmit}>
                <div className="form-field">
                    <label htmlFor="username">
                        Username
                    </label>

                    <input
                        id="username"
                        type="text"
                        value={username}
                        onChange={(event) =>
                            setUsername(event.target.value)
                        }
                        placeholder="johndoe"
                        autoComplete="username"
                        required
                    />
                </div>

                <div className="form-field">
                    <label htmlFor="register-email">
                        Email
                    </label>

                    <input
                        id="register-email"
                        type="email"
                        value={email}
                        onChange={(event) =>
                            setEmail(event.target.value)
                        }
                        placeholder="you@example.com"
                        autoComplete="email"
                        required
                    />
                </div>

                <div className="form-field">
                    <label htmlFor="register-password">
                        Password
                    </label>

                    <input
                        id="register-password"
                        type="password"
                        value={password}
                        onChange={(event) =>
                            setPassword(event.target.value)
                        }
                        placeholder="••••••••"
                        autoComplete="new-password"
                        required
                    />
                </div>

                <div className="form-field">
                    <label htmlFor="confirm-password">
                        Confirm password
                    </label>

                    <input
                        id="confirm-password"
                        type="password"
                        value={confirmPassword}
                        onChange={(event) =>
                            setConfirmPassword(event.target.value)
                        }
                        placeholder="••••••••"
                        autoComplete="new-password"
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
                    disabled={loading}
                >
                    {loading ? "Creating..." : "Create account"}
                </button>
            </form>

            <div className="auth-form-footer">
                <span>Already have an account?</span>

                <Link to="/auth/login">
                    Sign in
                </Link>
            </div>
        </div>
    );
};