import "./CommentCard.css";
import type {CommentResponse} from "../../../models/comments/responses/CommentResponse.ts";

interface CommentCardProps {
    comment: CommentResponse;
}

export const CommentCard = ({ comment }: CommentCardProps) => {
    const createdAt = new Date(comment.createdAt);

    return (
        <article className="comment-card">
            <div className="comment-avatar">
                {comment.authorName
                    .charAt(0)
                    .toUpperCase()}
            </div>

            <div className="comment-content">
                <div className="comment-header">
                    <span className="comment-author">
                        {comment.authorName}
                    </span>

                    <time className="comment-date">
                        {createdAt.toLocaleDateString()}
                    </time>
                </div>

                <p className="comment-text">
                    {comment.text}
                </p>
            </div>
        </article>
    );
};
