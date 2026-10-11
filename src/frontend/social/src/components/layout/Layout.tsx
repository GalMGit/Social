import { Outlet, useNavigate } from "react-router-dom";
import './Layout.css'


export const Layout = () => {
    const navigate = useNavigate();
    const isAuthenticated = !!localStorage.getItem("token");

    return (
        <div className="layout">
            <header className="site-header">
                <div className="header-container">
                    <button
                        className="logo"
                        onClick={() => navigate("/")}
                    >
                        <span className="logo-mark">
                            S
                        </span>

                        <span className="logo-text">
                            Social
                        </span>
                    </button>

                    <nav className="nav-menu">
                        {isAuthenticated ? (
                            <>
                                <button
                                    className="nav-item"
                                    onClick={() => navigate("/cart")}
                                >
                                    Создать пост
                                </button>

                                <button
                                    className="nav-item nav-item-danger"
                                    onClick={() => {
                                        localStorage.removeItem("token");
                                        navigate("/");
                                        window.location.reload();
                                    }}
                                >
                                    Выйти
                                </button>
                            </>
                        ) : (
                            <button
                                className="nav-item nav-item-primary"
                                onClick={() => navigate("/auth")}
                            >
                                Войти
                            </button>
                        )}
                    </nav>
                </div>
            </header>

            <main className="main-content">
                <Outlet />
            </main>
        </div>
    );
};