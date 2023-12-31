using System.Reflection.Metadata;

namespace Projet_A2_S1
{
    /// <summary>
    /// This class is dedicated to the lecture of the YAML file that stocks every player in a list of player
    /// </summary>
    public class PlayerList
    {
        private const string YAML_PATH = "data/config.yml";

        /// <summary>
        /// The playerlist is the only instance for this classe. It is only a list of player
        /// </summary>
        public List<Player> playerlist { get; set; }


        ///
        public PlayerList(List<Player> playerlist){
            this.playerlist=playerlist;
        }

        /// <summary>
        /// Write the information of the players in a YAML file.
        /// The 
        /// </summary>
        public void WriteYAML(){
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yamlString = serializer.Serialize(playerlist);

            File.WriteAllText(YAML_PATH, yamlString);
        }


        /// <summary>
        /// Read the YAML file and create a list of player
        /// </summary>
        public void ReadYAML()
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

            var yamlString = File.ReadAllText(YAML_PATH);
            playerlist = deserializer.Deserialize<List<Player>>(yamlString);
        }

        /// <summary>
        /// Function that return the information of every player
        /// </summary>
        /// <returns>Return a string that represent every player in the game
        /// </returns>
        public string toString(){
            string s="";
            foreach(Player player in playerlist){
                s=s+"\n"+player.toString();
            }
            return s;
        }

        /// <summary>
        /// Function that return the information of every player 
        /// </summary>
        /// <returns></returns>
        public List<string> toStringArray(){
            List<string> s = new ();
            foreach (var player in playerlist){
                s.Add(player.toString());
            }
            return s;
        }

    }


}