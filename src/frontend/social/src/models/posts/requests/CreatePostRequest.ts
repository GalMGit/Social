export interface CreatePostRequest {
    title: string;
    content: string;
    imagePath: string | null;
    communityId: string | null;
}