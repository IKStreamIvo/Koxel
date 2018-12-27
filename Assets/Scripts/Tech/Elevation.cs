using Newtonsoft.Json.Linq;

namespace Koxel.Modding
{
    public class Elevation
    {
        JToken jsonCalc;
        public Elevation(JToken jsonCalc){
            this.jsonCalc = jsonCalc;
        }

        public float Get(int x, int y){
            float value = new JSONCalc(jsonCalc, x, y).result;
            return value;
        }
    }
}