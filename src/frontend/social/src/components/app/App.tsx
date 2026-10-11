import { BrowserRouter as Router, Navigate, Route, Routes } from "react-router-dom";
import "./App.css";
import { Layout } from "../layout/Layout.tsx";
import {AuthLayout} from "../auth/auth-layout/AuthLayout.tsx";
import {LoginForm} from "../auth/login-form/LoginForm.tsx";
import {RegisterForm} from "../auth/register-form/RegisterForm.tsx";
import {Home} from "../home/Home.tsx";
import {PostView} from "../posts/post-view/PostView.tsx";


export const App = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route index element={<Home />} />

                    <Route
                        path="posts/:postId"
                        element={<PostView />}
                    />

                    <Route>
                        <Route path={"/auth"} element={<AuthLayout/>}>
                            <Route index element={<LoginForm/>}/>
                            <Route path={"login"} element={<LoginForm/>}/>
                            <Route path={"register"} element={<RegisterForm/>}/>
                        </Route>
                    </Route>

                </Route>


                <Route
                    path="*"
                    element={<Navigate to="/" replace />}
                />
            </Routes>
        </Router>
    );
};