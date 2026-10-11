import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./LoginForm.css";
import {authService} from "../../../services/auth-service/authService.ts";

export const LoginForm = () => {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        setError(null);
        setLoading(true);

        try {
            const response = await authService.login({
                email,
                password,
            });

            localStorage.setItem("token", response.data.token);

            navigate("/", { replace: true });
        } catch (error: any) {
            const message =
                error.response?.data?.message ??
                "Не удалось выполнить вход.";

            setError(message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-form">
            <div className="auth-form-header">
                <span className="auth-form-label">WELCOME BACK</span>
                <h1>Sign in</h1>
                <p>Enter your credentials to continue.</p>
            </div>

            <form onSubmit={handleSubmit}>
                <div className="form-field">
                    <label htmlFor="email">Email</label>

                    <input
                        id="email"
                        type="email"
                        value={email}
                        onChange={(event) => setEmail(event.target.value)}
                        placeholder="you@example.com"
                        autoComplete="email"
                        required
                    />
                </div>

                <div className="form-field">
                    <label htmlFor="password">Password</label>

                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                        placeholder="••••••••"
                        autoComplete="current-password"
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
                    {loading ? "Signing in..." : "Sign in"}
                </button>
            </form>

            <div className="auth-form-footer">
                <span>Don't have an account?</span>

                <Link to="/auth/register">
                    Create account
                </Link>
            </div>
        </div>
    );
};