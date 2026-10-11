export type PostResponse = {
    id: string;
    authorId: string;
    title: string;
    content: string;
    communityId?: string;
    imageUrl?: string;
    authorName: string;
    createdAt: Date;
}