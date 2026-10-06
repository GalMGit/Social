using Social.Comments.Application.DTOs;
using Social.Comments.Domain;

namespace Social.Comments.Application.Mappers;

public static class CommentMapper
{
    public static CommentResponse ToCommentResponse(
        this Comment comment,
        string? authorName)
        => new(
            comment.Id,
            comment.PostId,
            comment.Text, 
            comment.AuthorId,
            authorName,
            comment.CreatedAt);
}