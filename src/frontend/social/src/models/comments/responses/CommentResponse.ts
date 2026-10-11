export type CommentResponse = {
    id: string;
    postId: string;
    text: string;
    authorId:string;
    authorName: string;
    createdAt: Date;
}