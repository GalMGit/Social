import { useNavigate } from "react-router-dom";
import "./PostCard.css";
import type {PostResponse} from "../../../models/posts/responses/PostResponse.ts";

interface PostCardProps {
    post: PostResponse;
}

export const PostCard = ({ post }: PostCardProps) => {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate(`/posts/${post.id}`);
    };

    return (
        <article
            className="post-card"
            onClick={handleClick}
        >
            <div className="post-image-wrapper">
                {post.imageUrl ? (
                    <img
                        className="post-image"
                        src={post.imageUrl}
                        alt={""}
                    />
                ) : (
                    <div className="post-image-placeholder">
                        No image
                    </div>
                )}
            </div>

            <div className="post-info">
                <span className="post-label">
                    POST
                </span>
                <h4>{post.title}Р</h4>
                {/*<h3>*/}
                {/*    {product.name}*/}
                {/*</h3>*/}
            </div>
        </article>
    );
};