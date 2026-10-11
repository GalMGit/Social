import { apiService } from "../api-service/apiService.ts";
import type {CommentResponse} from "../../models/comments/responses/CommentResponse.ts";

export const commentService = {
    getByPostId: (postId: string) =>
        apiService.get<CommentResponse[]>(
            `comments/post/${postId}`
        ),
};