import "./PostList.css";
import type {PostResponse} from "../../../models/posts/responses/PostResponse.ts";
import {PostCard} from "../post-card/PostCard.tsx";

interface PostListProps {
    posts: PostResponse[];
}

export const PostList = ({ posts }: PostListProps) => {
    return (
        <section className="post-grid">
            {posts.map((post) => (
                <PostCard
                    post={post}
                    key={post.id}
                />
            ))}
        </section>
    );
};