import { apiService } from "../api-service/apiService.ts";
import type {PostResponse} from "../../models/posts/responses/PostResponse.ts";

export const postService = {
    getAll: () => {
        return apiService.get<PostResponse[]>(
            "posts");
    },
    getById: (postId: string) =>
        apiService.get<PostResponse>(
            `posts/${postId}`
        ),
};