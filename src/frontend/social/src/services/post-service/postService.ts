import { apiService } from "../api-service/apiService.ts";
import type {PostResponse} from "../../models/posts/responses/PostResponse.ts";
import type {CreatePostRequest} from "../../models/posts/requests/CreatePostRequest.ts";

export const postService = {
    getAll: () => {
        return apiService.get<PostResponse[]>(
            "posts");
    },
    getById: (postId: string) =>
        apiService.get<PostResponse>(
            `posts/${postId}`
        ),
    create: async (request: CreatePostRequest) => {
        const response = await apiService.post<string>(
            "posts",
            request
        );

        return response.data;
    },
};