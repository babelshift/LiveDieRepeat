using Newtonsoft.Json.Linq;
using SharpDL.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveDieRepeat.Content
{
	public class ContentManager
	{
		private readonly Renderer renderer;
		private const string contentRoot = "Content/";
		private const string contentDataRoot = contentRoot + "Data/";
		private const string fontReferencePath = contentDataRoot + "fonts.json";
		private const string textureReferencePath = contentDataRoot + "textures.json";

		private readonly Dictionary<string, string> fontPaths = new Dictionary<string, string>();
		private readonly Dictionary<string, string> texturePaths = new Dictionary<string, string>();
		
		public ContentManager(Renderer renderer)
		{
			this.renderer = renderer;

			LoadFontPaths(File.ReadAllText(fontReferencePath));
			LoadTexturePaths(File.ReadAllText(textureReferencePath));
		}

		private void LoadFontPaths(string jsonPath)
		{
			JObject o = JObject.Parse(jsonPath);
			foreach (var font in o["fonts"])
			{
				var keyValuePair = GetKeyValuePair(font);
				fontPaths.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void LoadTexturePaths(string jsonPath)
		{
			JObject o = JObject.Parse(jsonPath);
			foreach (var font in o["textures"])
			{
				var keyValuePair = GetKeyValuePair(font);
				fontPaths.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public Texture GetTexture(string texturePathKey)
		{
			if (texturePathKey == null) throw new ArgumentNullException("texturePathKey");

			string texturePath = GetTexturePath(texturePathKey);
			Surface surface = new Surface(texturePath, SurfaceType.PNG);
			Texture texture = new Texture(renderer, surface);
			return texture;
		}

		public TrueTypeText GetText(string fontPathKey, int fontSize, Color color, string text)
		{
			string fontPath = GetFontPath(fontPathKey);
			return TrueTypeTextFactory.CreateTrueTypeText(renderer, fontPath, fontSize, color, text, 0);
		}

		private string GetFontPath(string fontPathKey)
		{
			if (String.IsNullOrEmpty(fontPathKey)) throw new ArgumentNullException("key");

			string fontPath;
			if (fontPaths.TryGetValue(fontPathKey, out fontPath))
				return contentRoot + fontPath;

			throw new KeyNotFoundException(String.Format("The contentManager with key '{0}' was not found.", fontPathKey));
		}

		private string GetTexturePath(string texturePathKey)
		{
			if (String.IsNullOrEmpty(texturePathKey)) throw new ArgumentNullException("key");

			string textuerPath;
			if (fontPaths.TryGetValue(texturePathKey, out textuerPath))
				return contentRoot + textuerPath;

			throw new KeyNotFoundException(String.Format("The contentManager with key '{0}' was not found.", texturePathKey));
		}

		private KeyValuePair<string, string> GetKeyValuePair(JToken o)
		{
			if (o == null) throw new ArgumentNullException("o");

			string key = o["key"].ToString();
			string value = o["value"].ToString();

			return new KeyValuePair<string, string>(key, value);
		}

	}
}
