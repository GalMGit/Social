import { BrowserRouter as Router, Navigate, Route, Routes } from "react-router-dom";
import "./App.css";
import { Layout } from "../layout/Layout.tsx";
import {AuthLayout} from "../auth/auth-layout/AuthLayout.tsx";
import {LoginForm} from "../auth/login-form/LoginForm.tsx";
import {RegisterForm} from "../auth/register-form/RegisterForm.tsx";
import {Home} from "../home/Home.tsx";
import {PostView} from "../posts/post-view/PostView.tsx";
import PostCreate from "../posts/post-create/PostCreate.tsx";
import {ConfirmEmail} from "../auth/confirmemail-form/ConfirmEmail.tsx";


export const App = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route index element={<Home />} />

                    <Route path="posts/:postId" element={<PostView />}/>
                    <Route path="/posts/create" element={<PostCreate />} />

                    <Route>
                        <Route path={"/auth"} element={<AuthLayout/>}>
                            <Route index element={<LoginForm/>}/>
                            <Route path={"login"} element={<LoginForm/>}/>
                            <Route path={"register"} element={<RegisterForm/>}/>
                            <Route path={"confirm-email"} element={<ConfirmEmail/>}/>
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