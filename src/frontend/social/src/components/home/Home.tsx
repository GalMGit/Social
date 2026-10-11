import { useEffect, useState } from "react";
import "./Home.css";
import type {PostResponse} from "../../models/posts/responses/PostResponse.ts";
import {postService} from "../../services/post-service/postService.ts";
import {PostList} from "../posts/post-list/PostList.tsx";

export const Home = () => {
    const [posts, setPosts] = useState<PostResponse[]>([]);

    useEffect(() => {
        const getPosts = async () => {
            const response = await postService.getAll();
            setPosts(response.data);
        };

        getPosts();
    }, []);

    return (
        <div className="home">
            <header className="home-header">
                <div>
                    <h1>POSTS</h1>
                </div>

                <div className="home-count">
                    {posts.length} posts
                </div>
            </header>

            <PostList posts={posts} />
        </div>
    );
};