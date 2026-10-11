import { useEffect, useState } from "react";

import "./CommentList.css";
import type {CommentResponse} from "../../../models/comments/responses/CommentResponse.ts";
import {commentService} from "../../../services/comment-service/commentService.ts";
import {CommentCard} from "../comment-card/CommentCard.tsx";

interface CommentListProps {
    postId: string;
}

export const CommentList = ({ postId }: CommentListProps) => {
    const [comments, setComments] =
        useState<CommentResponse[]>([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        const loadComments = async () => {
            try {
                setLoading(true);
                setError(null);

                const response =
                    await commentService.getByPostId(postId);

                setComments(response.data);
            } catch (error) {
                console.error(error);
                setError("Failed to load comments");
            } finally {
                setLoading(false);
            }
        };

        loadComments();
    }, [postId]);

    return (
        <section className="comment-list">
            <div className="comment-list-header">
                <span className="comment-list-label">
                    COMMENTS
                </span>

                <h2>
                    {comments.length}{" "}
                    {comments.length === 1
                        ? "comment"
                        : "comments"}
                </h2>
            </div>

            {loading && (
                <div className="comment-list-status">
                    Loading comments...
                </div>
            )}

            {error && !loading && (
                <div className="comment-list-status">
                    {error}
                </div>
            )}

            {!loading &&
                !error &&
                comments.length === 0 && (
                    <div className="comment-list-status">
                        No comments yet.
                    </div>
                )}

            {!loading &&
                !error &&
                comments.length > 0 && (
                    <div className="comment-items">
                        {comments.map((comment) => (
                            <CommentCard
                                key={comment.id}
                                comment={comment}
                            />
                        ))}
                    </div>
                )}
        </section>
    );
};
