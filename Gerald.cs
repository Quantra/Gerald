using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Blish_HUD.GameService;
using Gerald.Controls;

namespace Gerald
{
	[Export(typeof(Blish_HUD.Modules.Module))]
	public class Gerald : Blish_HUD.Modules.Module
	{

		private static readonly Logger Logger = Logger.GetLogger<Module>();

		#region Service Managers
		internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
		internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
		internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
		internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
		#endregion

		[ImportingConstructor]
		public Gerald([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) { }

		private SettingEntry<int> _settingCursorThickness;

		protected override void DefineSettings(SettingCollection settings)
		{
			_settingCursorThickness = settings.DefineSetting("CursorThickness", 3, "Cursor Thickness");
            _settingCursorThickness.SetRange(1, 20);  // This doesn't seem to work?

            _settingCursorThickness.SettingChanged += UpdateCursorThickness;

        }

		private Texture2D dummyTexture;
		private Cursor lineH;
		private Cursor lineV;
		private bool updateCursor = true;
		private int skipUpdates = 0;

		protected override void Initialize()
		{
			dummyTexture = new Texture2D(Graphics.GraphicsDevice, 1, 1);
			dummyTexture.SetData(new Color[] { Color.Magenta });

			lineH = new Cursor
			{
				Parent = Graphics.SpriteScreen,
				Size = new Point(1920, _settingCursorThickness.Value),
				Location = new Point(0, 0),
				Texture = dummyTexture,
				Opacity = 1.0f
			};

			lineV = new Cursor
			{
				Parent = Graphics.SpriteScreen,
				Size = new Point(_settingCursorThickness.Value, 1080),
				Location = new Point(0, 0),
				Texture = dummyTexture,
				Opacity = 1.0f
			};

            Input.Mouse.LeftMouseButtonPressed += DisableCursorUpdates;
			Input.Mouse.LeftMouseButtonReleased += EnableCursorUpdates;

            Input.Mouse.RightMouseButtonPressed += DisableCursorUpdates;
            Input.Mouse.RightMouseButtonReleased += EnableCursorUpdates;
        }

		protected override async Task LoadAsync()
		{

		}

		protected override void OnModuleLoaded(EventArgs e)
		{

			// Base handler must be called
			base.OnModuleLoaded(e);
		}

		protected override void Update(GameTime gameTime)
		{
            UpdateCursorPosition();
        }

		/// <inheritdoc />
		protected override void Unload()
		{
			// Unload here

			// All static members must be manually unset
		}


        private void UpdateCursorPosition()
        {
			if (!updateCursor || Input.Mouse.MouseHidden) { return; }

			if (skipUpdates > 0)
			{
				skipUpdates -= 1;
				return;
			}

			Point mouseLoc = Input.Mouse.Position;

            lineH.Location = new Point(lineH.Location.X, mouseLoc.Y - _settingCursorThickness.Value / 2);
			lineV.Location = new Point(mouseLoc.X - _settingCursorThickness.Value / 2, lineV.Location.Y);
		}

		private void UpdateCursorThickness(object sender = null, ValueChangedEventArgs<int> e = null)
		{
			lineH.Size = new Point(lineH.Size.X, _settingCursorThickness.Value);
			lineV.Size = new Point(_settingCursorThickness.Value, lineV.Size.Y);
		}

		private void DisableCursorUpdates(object sender, MouseEventArgs e)
        {
			updateCursor = false;
			skipUpdates = 20;  // Min value of 3 seems to work. Higher = more stutter but less jumps.
		}

		private void EnableCursorUpdates(object sender, MouseEventArgs e)
        {
			updateCursor = true;
        }

	}

}
