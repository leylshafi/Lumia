namespace Lumia.Utilities.Extentions
{
	public static class FileValidation
	{
		public static bool ValdiateType(this IFormFile file, string type = "image/")
		{
			if(file.ContentType.Contains(type))
			{
				return true;
			}
			return false;
		}
		public static bool ValdiateSize(this IFormFile file, int limitKb=2)
		{
			if (file.Length<=limitKb*1024*1024)
			{
				return true;
			}
			return false;
		}

		private static string CreatePath(string root, string fileName, params string[] folders)
		{
			string path = root;
			foreach (var item in folders)
			{
				path = Path.Combine(path, item);
			}
			path = Path.Combine(path, fileName);
			return path;
		}

		public async static Task<string> CreateFile(this IFormFile file,string root, params string[] folders)
		{
			string extention = Path.GetExtension(file.FileName);
			string fileName = $"{Guid.NewGuid()}{extention}";
			string path = CreatePath(root, fileName, folders);

			using(FileStream fs = new FileStream(path,FileMode.Create))
			{
				await file.CopyToAsync(fs);
			}
			return fileName;
		}

		public static async Task DeleteFile(this string fileName, string root, params string[] folders)
		{
			string path = CreatePath(root, fileName, folders);
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}
