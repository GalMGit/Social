import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import "./PostView.css";
import type {PostResponse} from "../../../models/posts/responses/PostResponse.ts";
import {postService} from "../../../services/post-service/postService.ts";
import {CommentList} from "../../comments/comment-list/CommentList.tsx";

export const PostView = () => {
    const { postId } = useParams();
    const navigate = useNavigate();

    const [post, setPost] =
        useState<PostResponse | null>(null);

    const [loading, setLoading] = useState(true);
    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        if (!postId) {
            return;
        }

        const loadPost = async () => {
            try {
                setLoading(true);
                setError(null);

                const response =
                    await postService.getById(postId);

                setPost(response.data);
            } catch (error) {
                console.error(error);
                setError("Failed to load post");
            } finally {
                setLoading(false);
            }
        };

        loadPost();
    }, [postId]);


    if (loading) {
        return (
            <main className="post-view">
                <div className="post-view-status">
                    Loading...
                </div>
            </main>
        );
    }

    if (error || !post) {
        return (
            <main className="post-view">
                <div className="post-view-status">
                    <p>
                        {error ?? "Post not found"}
                    </p>

                    <button
                        onClick={() => navigate(-1)}
                    >
                        Go back
                    </button>
                </div>
            </main>
        );
    }

    return (
        <main className="post-view">
            <button
                className="post-back-button"
                onClick={() => navigate(-1)}
            >
                ← Back
            </button>

            <section className="post-details">
                <div className="post-details-image">
                    {post.imageUrl ? (
                        <img
                            src={post.imageUrl}
                            alt={""}
                        />
                    ) : (
                        <div className="post-details-placeholder">
                            No image
                        </div>
                    )}
                </div>

                <div className="post-details-content">
                    <span className="post-details-label">
                        POST
                    </span>

                    <h1>{post.title}</h1>

                    <p className="post-details-description">
                        {post.content}
                    </p>
                </div>
            </section>
            <CommentList postId={post.id} />
        </main>
    );
};