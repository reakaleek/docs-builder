namespace Documentation.Builder.LinkIndex;

public interface ILinkIndex
{
    Task UploadFileAsync(string filePath, bool shouldUpload);
} 
