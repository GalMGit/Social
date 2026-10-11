import type {LoginRequest} from "../../models/auth/login/requests/LoginRequest.ts";
import {apiService} from "../api-service/apiService.ts";
import type {LoginResponse} from "../../models/auth/login/responses/LoginResponse.ts";
import type {RegisterRequest} from "../../models/auth/register/requests/RegisterRequest.ts";

export const authService = {
    login: (data: LoginRequest) =>
        apiService.post<LoginResponse>("auth/login", data),

    register: (data: RegisterRequest) =>
        apiService.post("auth/register", data),
}