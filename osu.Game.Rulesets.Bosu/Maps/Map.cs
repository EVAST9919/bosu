using osu.Game.Rulesets.Bosu.UI;
using System;
using System.Linq;

namespace osu.Game.Rulesets.Bosu.Maps
{
    public abstract class Map
    {
        private readonly string playfield;

        public Map()
        {
            playfield = CreatePlayfield();

            if (playfield.Length != BosuPlayfield.TILES_HEIGHT * BosuPlayfield.TILES_WIDTH)
                throw new IndexOutOfRangeException("Playfield size is incorrect");
        }

        public char GetTileAt(int x, int y)
        {
            if (x >= BosuPlayfield.TILES_WIDTH || x < 0 || y >= BosuPlayfield.TILES_HEIGHT || y < 0)
                throw new IndexOutOfRangeException("Incorrect input parameters");

            return playfield.ElementAt(y * BosuPlayfield.TILES_WIDTH + x);
        }

        protected abstract string CreatePlayfield();
    }
}
