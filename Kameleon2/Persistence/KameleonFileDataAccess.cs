using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Kameleon2.Persistence
{
    public class KameleonFileDataAccess : IKameleonDataAccess
    {
        public async Task SaveAsync(string path, KameleonMap map)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) 
                {
                    writer.WriteLine(map.MapSize);
                    
                    for (int i = 0; i < map.MapSize; i++)
                    {
                        for (int j = 0; j < map.MapSize; j++)
                        {
                            await writer.WriteAsync(map.getFieldsColor(i,j) + " "); 
                        }
                        await writer.WriteLineAsync();
                    }

                    for (int i = 0; i < map.MapSize; i++)
                    {
                        for (int j = 0; j < map.MapSize; j++)
                        {
                            await writer.WriteAsync(map.getFieldsPlayer(i,j) + " "); 
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new KameleonDataException();
            }
        }


        public async Task<KameleonMap> LoadAsync(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path)) 
                {
                    String line = await reader.ReadLineAsync();

                    int size = Convert.ToInt32(line); 

                    

                    KameleonMap map = new KameleonMap(size); 

                    for (int i = 0; i < size; i++)
                    {
                        line = await reader.ReadLineAsync();

                        string[]colors = line.Split(' ');

                        for (int j = 0; j < size; j++)
                        {
                            if (colors[j]=="Red")
                            {
                                map.setFieldsColor(i,j,Color.Red);
                            }
                            else if (colors[j] == "Green")
                            {
                                map.setFieldsColor(i, j, Color.Green);
                            }
                            else map.setFieldsColor(i, j, Color.Empty);

                        }
                    }

                    for (int i = 0; i < size; i++)
                    {
                        line = await reader.ReadLineAsync();
                        string[] players = line.Split(' ');

                        for (int j = 0; j < size; j++)
                        {
                            if (players[j] == "Red")
                            {
                                map.setFieldsPlayer(i, j, Color.Red);
                            }
                            else if (players[j] == "Green")
                            {
                                map.setFieldsPlayer(i, j, Color.Green);
                            }
                            else map.setFieldsPlayer(i, j, Color.Empty);
                        }
                    }

                    return map;
                }
            }
            catch
            {
                throw new KameleonDataException();
            }
        }
    }
}
