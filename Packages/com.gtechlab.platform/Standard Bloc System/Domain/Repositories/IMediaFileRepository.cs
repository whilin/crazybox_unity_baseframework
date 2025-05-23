using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IMediaFileRepository {
    Task<string> UploadImage (string repoPath, string filename, byte[] imageBytes);
}