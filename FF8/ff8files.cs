﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FF8
{
    /// <summary>
    /// parse data from save game files
    /// </summary>
    /// <seealso cref="http://wiki.ffrtt.ru/index.php/FF8/GameSaveFormat#The_save_format"/>
    /// <seealso cref="https://github.com/myst6re/hyne"/>
    /// <seealso cref="https://cdn.discordapp.com/attachments/552838120895283210/570733614656913408/ff8_save.zip"/>
    /// <remarks>antiquechrono was helping. he even wrote a whole class using kaitai. Though I donno if we wanna use kaitai.</remarks>
    internal static class Ff8files
    {
        public struct Data
        {
            public ushort LocationID;//0x0004
            public ushort firstcharacterscurrentHP;//0x0006
            public ushort firstcharactersmaxHP;//0x0008
            public ushort savecount;//0x000A
            public uint AmountofGil;//0x000C
            public TimeSpan Totalnumberofsecondsplayed;//0x0020
            public byte firstcharacterslevel;//0x0024
            public byte firstcharactersportrait;//0x0025
            public byte secondcharactersportrait;//0x0026
            public byte thirdcharactersportrait;//0x0027
            public byte[] Squallsname;//0x0028 //12
            public byte[] Rinoasname;//0x0034 //12
            public byte[] Angelosname;//0x0040 //12
            public byte[] Bokosname;//0x004C
            public uint CurrentDisk;//0x0058
            public uint Currentsave;//0x005C
        }
        /// <summary>
        /// Locations used by save files.
        /// </summary>
        /// <seealso cref="https://github.com/myst6re/hyne/blob/master/Data.cpp"/>
        public static string[] Locations = new string[] {
 ("???")
,("Plaines d'Arkland - Balamb")
,("Monts Gaulg - Balamb")
,("Baie de Rinaul - Balamb")
,("Cap Raha - Balamb")
,("Forêt de Rosfall - Timber")
,("Mandy Beach - Timber")
,("Lac Obel - Timber")
,("Vallée de Lanker - Timber")
,("Ile Nantakhet - Timber")
,("Yaulny Canyon - Timber")
,("Val Hasberry - Dollet")
,("Cap Holy Glory - Dollet")
,("Longhorn Island - Dollet")
,("Péninsule Malgo - Dollet")
,("Plateau Monterosa - Galbadia")

,("Lallapalooza Canyon - Galbadia")
,("Shenand Hill - Timber")
,("Péninsule Gotland - Galbadia")
,("Ile de l'Enfer - Galbadia")
,("Plaine Galbadienne")
,("Wilburn Hill - Galbadia")
,("Archipel Rem - Galbadia")
,("Dingo Désert - Galbadia")
,("Cap Winhill")
,("Archipel Humphrey - Winhill")
,("Ile Winter - Trabia")
,("Val de Solvard - Trabia")
,("Crête d'Eldbeak - Trabia")
 ,("")
,("Plaine d'Hawkind - Trabia")
,("Atoll Albatross - Trabia")

,("Vallon de Bika - Trabia")
,("Péninsule Thor - Trabia")
 ,("")
,("Crête d'Heath - Trabia")
,("Trabia Crater - Trabia")
,("Mont Vienne - Trabia")
,("Plaine de Mordor - Esthar")
,("Mont Nortes - Esthar")
,("Atoll Fulcura - Esthar")
,("Forêt Grandidi - Esthar")
,("Iles Millefeuilles - Esthar")
,("Grandes plaines d'Esthar")
,("Esthar City")
,("Salt Lake - Esthar")
,("Côte Ouest - Esthar")
,("Mont Sollet - Esthar")

,("Vallée d'Abadan - Esthar")
,("Ile Minde - Esthar")
,("Désert Kashkabald - Esthar")
,("Ile Paradisiaque - Esthar")
,("Pic de Talle - Esthar")
,("Atoll Shalmal - Esthar")
,("Vallée de Lolestern - Centra")
,("Aiguille d'Almage - Centra")
,("Vallon Lenown - Centra")
,("Cap de l'espoir - Centra")
,("Mont Yorn - Centra")
,("Ile Pampa - Esthar")
,("Val Serengetti - Centra")
,("Péninsule Nectalle - Centra")
,("Centra Crater - Centra")
,("Ile Poccarahi - Centra")

,("Bibliothèque - BGU")
,("Entrée - BGU")
,("Salle de cours - BGU")
,("Cafétéria - BGU")
,("Niveau MD - BGU")
,("Hall 1er étage - BGU")
,("Hall - BGU")
,("Infirmerie - BGU")
,("Dortoirs doubles - BGU")
,("Dortoirs simples - BGU")
,("Bureau proviseur - BGU")
,("Parking - BGU")
,("Salle de bal - BGU")
,("Campus - BGU")
,("Serre de combat - BGU")
,("Zone secrète - BGU")

,("Corridor - BGU")
,("Temple - BGU")
,("Pont - BGU")
,("Villa Dincht - Balamb")
,("Hôtel - Balamb")
,("Place centrale - Balamb")
,("Place de la gare - Balamb")
,("Port - Balamb")
,("Résidence - Balamb")
,("Train")
,("Voiture")
,("Vaisseau")
,("Mine de souffre")
,("Place du village - Dollet")
,("Zuma Beach")
,("Port - Dollet")

,("Pub - Dollet")
,("Hôtel - Dollet")
,("Résidence - Dollet")
,("Tour satellite - Dollet")
,("Refuge montagneux - Dollet")
,("Centre ville - Timber")
,("Chaîne de TV - Timber")
,("Base des Hiboux - Timber")
,("Pub - Timber")
,("Hôtel - Timber")
,("Train - Timber")
,("Résidence - Timber")
,("Ecran géant - Timber")
,("Centre de presse - Timber")
,("Forêt de Timber")
,("Entrée - Fac de Galbadia")

,("Station Fac de Galbadia")
,("Hall - Fac de Galbadia")
,("Corridor - Fac de Galbadia")
,("Salle d'attente - Fac Galbadia")
,("Salle de cours - Fac Galbadia")
,("Salle de réunion - Fac Galbadia")
,("Dortoirs - Fac de Galbadia")
,("Ascenseur - Fac de Galbadia")
,("Salle recteur - Fac Galbadia")
,("Auditorium - Fac de Galbadia")
,("Stade - Fac de Galbadia")
,("Stand - Fac de Galbadia")
,("2nde entrée - Fac Galbadia")
,("Gymnase - Fac de Galbadia")
,("Palais président - Deling City")
,("Manoir Caraway - Deling City")

,("Gare - Deling City")
,("Place centrale - Deling City")
,("Hôtel - Deling City")
,("Bar - Deling City")
,("Sortie - Deling City")
,("Parade - Deling City")
,("Egout - Deling City")
,("Prison du désert - Galbadia")
,("Désert")
,("Base des missiles")
,("Village de Winhill")
,("Pub - Winhill")
,("Maison vide - Winhill")
,("Manoir - Winhill")
,("Résidence - Winhill")
,("Hôtel - Winhill")

,("Voiture - Winhill")
,("Tombe du roi inconnu")
,("Horizon")
,("Habitations - Horizon")
,("Ecrans solaires - Horizon")
,("Villa du maire - Horizon")
,("Usine - Horizon")
,("Salle des fêtes - Horizon")
,("Hôtel - Horizon")
,("Résidence - Horizon")
,("Gare - Horizon")
,("Aqueduc d'Horizon")
,("Station balnéaire")
,("Salt Lake")
,("Bâtiment mystérieux")
,("Esthar City")

,("Laboratoire Geyser - Esthar")
,("Aérodrome - Esthar")
,("Lunatic Pandora approche")
,("Résidence président - Esthar")
,("Hall - Résidence président")
,("Couloir - Résidence président")
,("Bureau - Résidence président")
,("Accueil - Labo Geyser")
,("Laboratoire Geyser")
,("Deleted")
,("Lunar Gate")
,("Parvis - Lunar Gate")
,("Glacière - Lunar gate")
,("Mausolée - Esthar")
,("Entrée - Mausolée")
,("Pod de confinement - Mausolée")

,("Salle de contrôle - Mausolée")
,("Tears Point")
,("Labo Lunatic Pandora")
,("Zone d'atterrissage de secours")
,("Zone d'atterrissage officielle")
,("Lunatic Pandora")
,("Site des fouilles - Centra")
,("Orphelinat")
,("Salle de jeux - Orphelinat")
,("Dortoir - Orphelinat")
,("Jardin - Orphelinat")
,("Front de mer - Orphelinat")
,("Champ - Orphelinat")
,("Ruines de Centra")
,("Entrée - Fac de Trabia")
,("Cimetière - Fac de Trabia")

,("Garage - Fac de Trabia")
,("Campus - Fac Trabia")
,("Amphithéatre - Fac de Trabia")
,("Stade - Fac de Trabia")
,("Dôme mystérieux")
,("Ville du désert - Shumi Village")
,("Ascenseur - Shumi Village")
,("Shumi Village")
,("Habitation - Shumi Village")
,("Résidence - Shumi Village")
,("Habitat - Shumi Village")
,("Hôtel - Shumi Village")
,("Trabia canyon")
,("Vaisseau des Seeds blancs")
,("Navire des Seeds Blancs")
,("Cabine - Navire Seeds blancs")

,("Cockpit - Hydre")
,("Siège passager - Hydre")
,("Couloir - Hydre")
,("Hangar - Hydre")
,("Accès - Hydre")
,("Air Room - Hydre")
,("Salle de pression - Hydre")
,("Centre de recherches Deep Sea")
,("Laboratoire - Deep Sea")
,("Salle de travail - Deep Sea")
,("Fouilles - Deep Sea")
,("Salle de contrôle - Base lunaire")
,("Centre médical - Base lunaire")
,("Pod - Base lunaire")
,("Dock - Base lunaire")
,("Passage - Base lunaire")

,("Vestiaire - Base lunaire")
,("Habitats - Base lunaire")
,("Hyper Espace")
,("Forêt Chocobo")
,("Jungle")
,("Citadelle d'Ultimecia - Vestibule")
,("Citadelle d'Ultimecia - Hall")
,("Citadelle d'Ultimecia - Terrasse")
,("Citadelle d'Ultimecia - Cave")
,("Citadelle d'Ultimecia - Couloir")
,("Elévateur - Citadelle")
,("Escalier - Citadelle d'Ultimecia")
,("Salle du trésor - Citadelle")
,("Salle de rangement - Citadelle")
,("Citadelle d'Ultimecia - Galerie")
,("Citadelle d'Ultimecia - Ecluse")

,("Citadelle - Armurerie")
,("Citadelle d'Ultimecia - Prison")
,("Citadelle d'Ultimecia - Fossé")
,("Citadelle d'Ultimecia - Jardin")
,("Citadelle d'Ultimecia - Chapelle")
,("Clocher - Citadelle d'Ultimecia")
,("Chambre d'Ultimecia - Citadelle")
 ,("???")
,("Citadelle d'Ultimecia")
,("Salle d'initiation")
,("Reine des cartes")
 ,("???")
 ,("???")
 ,("???")
 ,("???")
 ,("???")
    };


        //C:\Users\pcvii\OneDrive\Documents\Square Enix\FINAL FANTASY VIII Steam\user_1727519
        // might crash linux. just trying to get something working.
        public static string SaveFolder { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Square Enix", "FINAL FANTASY VIII Steam");
        public static Data[,] FileList { get; private set; }

        public static void init()
        {
            SaveFolder = Directory.GetDirectories(SaveFolder)[0];
            FileList = new Data[2, 30];
            foreach (string file in Directory.EnumerateFiles(SaveFolder))
            {
                Match n = Regex.Match(file, @"slot(\d+)_save(\d+).ff8");
                    
                if(n.Success && n.Groups.Count>0)
                {
                    FileList[int.Parse(n.Groups[1].Value)-1, int.Parse(n.Groups[2].Value)-1] = read(file);
                }
            }
        }

        private static Data read(string file)
        {
            byte[] decmp;

            using (FileStream fs = File.OpenRead(file))
            using (BinaryReader br = new BinaryReader(fs))
            {
                uint size = br.ReadUInt32();
                //uint fsLen = BitConverter.ToUInt32(FI, loc * 12);
                //uint fSpos = BitConverter.ToUInt32(FI, (loc * 12) + 4);
                //bool compe = BitConverter.ToUInt32(FI, (loc * 12) + 8) != 0;
                //fs.Seek(0, SeekOrigin.Begin);
                byte[] tmp = br.ReadBytes((int)fs.Length-4);
                decmp = LZSS.DecompressAllNew(tmp);
            }
            //using (FileStream fs = File.Create(Path.Combine(@"d:\", Path.GetFileName(file))))
            //using (BinaryWriter bw = new BinaryWriter(fs))
            //{
            //    bw.Write(decmp);
            //}
            using (MemoryStream ms = new MemoryStream(decmp))
            using (BinaryReader br = new BinaryReader(ms))
            {
                Data d = new Data();
                ms.Seek(0x184, SeekOrigin.Begin);
                d.LocationID = br.ReadUInt16();//0x0004
                d.firstcharacterscurrentHP = br.ReadUInt16();//0x0006
                d.firstcharactersmaxHP = br.ReadUInt16();//0x0008
                d.savecount = br.ReadUInt16();//0x000A
                d.AmountofGil = br.ReadUInt32();//0x000C
                d.Totalnumberofsecondsplayed = new TimeSpan(0,0,(int)br.ReadUInt32());//0x0020
                d.firstcharacterslevel = br.ReadByte();//0x0024
                d.firstcharactersportrait = br.ReadByte();//0x0025
                d.secondcharactersportrait = br.ReadByte();//0x0026
                d.thirdcharactersportrait = br.ReadByte();//0x0027
                d.Squallsname = br.ReadBytes(12);//0x0028
                d.Rinoasname = br.ReadBytes(12);//0x0034
                d.Angelosname = br.ReadBytes(12);//0x0040
                d.Bokosname = br.ReadBytes(12);//0x004C
                d.CurrentDisk = br.ReadUInt32();//0x0058
                d.Currentsave = br.ReadUInt32();//0x005C
                return d;
            }
        }
    }
}