using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedMerchantList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // A merchant a user created by hand may share a normalized name with one of the rows
            // below. Lift the unique constraint so the insert can't fail mid-deploy; MergeDuplicates
            // folds any such pair together afterwards and the index goes straight back on.
            migrationBuilder.DropIndex(
                name: "IX_Merchants_NormalizedName",
                table: "Merchants");

            migrationBuilder.InsertData(
                table: "MerchantAliases",
                columns: new[] { "Id", "MerchantId", "NormalizedAlias" },
                values: new object[,]
                {
                    { 148, 5, "aldilocal" },
                    { 149, 6, "lidlplus" },
                    { 150, 7, "localcoop" },
                    { 151, 18, "londisexpress" },
                    { 152, 16, "sparexpress" },
                    { 153, 19, "premierexpress" },
                    { 154, 17, "costcutterexpress" },
                    { 155, 15, "nisalocal" },
                    { 156, 4, "morrisonsmarketkitchen" },
                    { 157, 109, "selfridgesfoodhall" },
                    { 158, 3, "asdapetrol" },
                    { 159, 4, "morrisonspetrol" },
                    { 160, 2, "sainsburyspetrol" },
                    { 161, 1, "tescopetrol" },
                    { 162, 20, "costcopetrol" },
                    { 163, 26, "shellrecharge" },
                    { 164, 35, "kwikfitmobile" },
                    { 165, 43, "caffeneroexpress" },
                    { 166, 43, "neroexpress" },
                    { 167, 63, "zizziexpress" },
                    { 168, 61, "yosushiexpress" },
                    { 169, 60, "wasabisushi" },
                    { 170, 64, "askitalianexpress" },
                    { 171, 41, "costaexpressdrivethru" },
                    { 172, 132, "screwfixtrade" },
                    { 173, 135, "toolstationtrade" },
                    { 174, 131, "tradepointbq" },
                    { 175, 141, "wilkohome" }
                });

            migrationBuilder.InsertData(
                table: "Merchants",
                columns: new[] { "Id", "Name", "NormalizedName", "Website" },
                values: new object[,]
                {
                    { 1001, "Amazon Fresh", "amazonfresh", "amazon.co.uk" },
                    { 1002, "Bargain Booze", "bargainbooze", "bargainbooze.co.uk" },
                    { 1003, "Bestway", "bestway", "bestwaywholesale.co.uk" },
                    { 1004, "Central England Co-op", "centralenglandcoop", "centralengland.coop" },
                    { 1005, "Country Range", "countryrange", "countryrange.co.uk" },
                    { 1006, "East of England Co-op", "eastofenglandcoop", "eastofengland.coop" },
                    { 1007, "Fultons Foods", "fultonsfoods", "fultonsfoods.co.uk" },
                    { 1008, "Heron Foods", "heronfoods", "heronfoods.com" },
                    { 1009, "HISBE", "hisbe", "hisbe.co.uk" },
                    { 1010, "Jack's", "jacks", "jacks.co.uk" },
                    { 1011, "Keystore", "keystore", "keystoreconvenience.co.uk" },
                    { 1012, "McColl's", "mccolls", "mccolls.co.uk" },
                    { 1013, "Midcounties Co-operative", "midcountiescooperative", "midcounties.coop" },
                    { 1014, "One Stop", "onestop", "onestop.co.uk" },
                    { 1015, "Parfetts", "parfetts", "parfetts.co.uk" },
                    { 1016, "Poundbakery", "poundbakery", "poundbakery.co.uk" },
                    { 1017, "Riverford", "riverford", "riverford.co.uk" },
                    { 1018, "Scotmid", "scotmid", "scotmid.coop" },
                    { 1019, "Simply Fresh", "simplyfresh", "simplyfresh.co.uk" },
                    { 1020, "Sunday Market", "sundaymarket", "sundaymarket.co.uk" },
                    { 1021, "The Food Warehouse", "thefoodwarehouse", "thefoodwarehouse.com" },
                    { 1022, "Today's Local", "todayslocal", "todays.co.uk" },
                    { 1023, "Abel & Cole", "abelcole", "abelandcole.co.uk" },
                    { 1024, "Approved Food", "approvedfood", "approvedfood.co.uk" },
                    { 1025, "HelloFresh", "hellofresh", "hellofresh.co.uk" },
                    { 1026, "Mindful Chef", "mindfulchef", "mindfulchef.com" },
                    { 1027, "Muscle Food", "musclefood", "musclefood.com" },
                    { 1028, "Oddbox", "oddbox", "oddbox.co.uk" },
                    { 1029, "BP Pulse", "bppulse", "bppulse.co.uk" },
                    { 1030, "Certas Energy", "certasenergy", "certasenergy.co.uk" },
                    { 1031, "Harvest Energy", "harvestenergy", "harvestenergy.co.uk" },
                    { 1032, "Instavolt", "instavolt", "instavolt.co.uk" },
                    { 1033, "Ionity", "ionity", "ionity.eu" },
                    { 1034, "Motor Fuel Group", "motorfuelgroup", "motorfuelgroup.com" },
                    { 1035, "Osprey Charging", "ospreycharging", "ospreycharging.co.uk" },
                    { 1036, "Pod Point", "podpoint", "pod-point.com" },
                    { 1037, "Tesla Supercharger", "teslasupercharger", "tesla.com" },
                    { 1038, "Arnold Clark", "arnoldclark", "arnoldclark.com" },
                    { 1039, "Autoglass", "autoglass", "autoglass.co.uk" },
                    { 1040, "Bristol Street Motors", "bristolstreetmotors", "bristolstreet.co.uk" },
                    { 1041, "Car Shop", "carshop", "carshop.co.uk" },
                    { 1042, "Cazoo", "cazoo", "cazoo.co.uk" },
                    { 1043, "Cinch", "cinch", "cinch.co.uk" },
                    { 1044, "Evans Halshaw", "evanshalshaw", "evanshalshaw.com" },
                    { 1045, "Formula One Autocentres", "formulaoneautocentres", "f1autocentres.co.uk" },
                    { 1046, "GSF Car Parts", "gsfcarparts", "gsfcarparts.com" },
                    { 1047, "Hendy Group", "hendygroup", "hendy.co.uk" },
                    { 1048, "Jardine Motors", "jardinemotors", "jardinemotors.co.uk" },
                    { 1049, "Lookers", "lookers", "lookers.co.uk" },
                    { 1050, "Marshall Motor Group", "marshallmotorgroup", "marshall.co.uk" },
                    { 1051, "Micheldever Tyres", "micheldevertyres", "michelde.co.uk" },
                    { 1052, "Motorpoint", "motorpoint", "motorpoint.co.uk" },
                    { 1053, "Mr Clutch", "mrclutch", "mrclutch.com" },
                    { 1054, "National Windscreens", "nationalwindscreens", "nationalwindscreens.co.uk" },
                    { 1055, "Pendragon", "pendragon", "pendragonplc.com" },
                    { 1056, "Protyre", "protyre", "protyre.co.uk" },
                    { 1057, "Sytner", "sytner", "sytner.co.uk" },
                    { 1058, "Tyre Shopper", "tyreshopper", "tyreshopper.co.uk" },
                    { 1059, "Vertu Motors", "vertumotors", "vertumotors.com" },
                    { 1060, "Webuyanycar", "webuyanycar", "webuyanycar.com" },
                    { 1061, "Green Flag", "greenflag", "greenflag.com" },
                    { 1062, "Britannia Rescue", "britanniarescue", "britanniarescue.com" },
                    { 1063, "Start Rescue", "startrescue", "startrescue.co.uk" },
                    { 1064, "DVLA", "dvla", "gov.uk" },
                    { 1065, "Dart Charge", "dartcharge", "gov.uk" },
                    { 1066, "M6 Toll", "m6toll", "m6toll.co.uk" },
                    { 1067, "Abercrombie & Fitch", "abercrombiefitch", "abercrombie.com" },
                    { 1068, "Alexander McQueen", "alexandermcqueen", "alexandermcqueen.com" },
                    { 1069, "All Saints", "allsaints", "allsaints.com" },
                    { 1070, "Ann Summers", "annsummers", "annsummers.com" },
                    { 1071, "Aquascutum", "aquascutum", "aquascutum.com" },
                    { 1072, "Arket", "arket", "arket.com" },
                    { 1073, "Barbour", "barbour", "barbour.com" },
                    { 1074, "Bershka", "bershka", "bershka.com" },
                    { 1075, "Berghaus", "berghaus", "berghaus.com" },
                    { 1076, "Blacks", "blacks", "blacks.co.uk" },
                    { 1077, "Boden", "boden", "boden.co.uk" },
                    { 1078, "Bonmarche", "bonmarche", "bonmarche.co.uk" },
                    { 1079, "Burberry", "burberry", "burberry.com" },
                    { 1080, "Burton", "burton", "burton.co.uk" },
                    { 1081, "Calvin Klein", "calvinklein", "calvinklein.co.uk" },
                    { 1082, "Charles Tyrwhitt", "charlestyrwhitt", "charlestyrwhitt.com" },
                    { 1083, "Clarks", "clarks", "clarks.co.uk" },
                    { 1084, "Coast", "coast", "coastfashion.com" },
                    { 1085, "Cos", "cos", "cos.com" },
                    { 1086, "Cotswold Outdoor", "cotswoldoutdoor", "cotswoldoutdoor.com" },
                    { 1087, "Crew Clothing", "crewclothing", "crewclothing.co.uk" },
                    { 1088, "Deichmann", "deichmann", "deichmann.com" },
                    { 1089, "Dorothy Perkins", "dorothyperkins", "dorothyperkins.com" },
                    { 1090, "Dr Martens", "drmartens", "drmartens.com" },
                    { 1091, "Dune London", "dunelondon", "dunelondon.com" },
                    { 1092, "Ede & Ravenscroft", "ederavenscroft", "edeandravenscroft.com" },
                    { 1093, "Fat Face", "fatface", "fatface.com" },
                    { 1094, "Foot Asylum", "footasylum", "footasylum.com" },
                    { 1095, "Foot Locker", "footlocker", "footlocker.co.uk" },
                    { 1096, "French Connection", "frenchconnection", "frenchconnection.com" },
                    { 1097, "Gap", "gap", "gap.co.uk" },
                    { 1098, "Gant", "gant", "gant.co.uk" },
                    { 1099, "George at Asda", "georgeatasda", "asda.com" },
                    { 1100, "Gymshark", "gymshark", "gymshark.com" },
                    { 1101, "Hackett", "hackett", "hackett.com" },
                    { 1102, "Harvey Nichols", "harveynichols", "harveynichols.com" },
                    { 1103, "Hobbs", "hobbs", "hobbs.com" },
                    { 1104, "Hollister", "hollister", "hollisterco.com" },
                    { 1105, "Hotter Shoes", "hottershoes", "hotter.com" },
                    { 1106, "House of CB", "houseofcb", "houseofcb.com" },
                    { 1107, "Hugo Boss", "hugoboss", "hugoboss.com" },
                    { 1108, "Jack Wills", "jackwills", "jackwills.com" },
                    { 1109, "Jaeger", "jaeger", "jaeger.co.uk" },
                    { 1110, "Jigsaw", "jigsaw", "jigsaw-online.com" },
                    { 1111, "Joules", "joules", "joules.com" },
                    { 1112, "Karen Millen", "karenmillen", "karenmillen.com" },
                    { 1113, "Kurt Geiger", "kurtgeiger", "kurtgeiger.com" },
                    { 1114, "Lacoste", "lacoste", "lacoste.com" },
                    { 1115, "Levi's", "levis", "levi.com" },
                    { 1116, "Lipsy", "lipsy", "lipsy.co.uk" },
                    { 1117, "Long Tall Sally", "longtallsally", "longtallsally.com" },
                    { 1118, "Lululemon", "lululemon", "lululemon.co.uk" },
                    { 1119, "Mango", "mango", "shop.mango.com" },
                    { 1120, "Mint Velvet", "mintvelvet", "mintvelvet.com" },
                    { 1121, "Missguided", "missguided", "missguided.co.uk" },
                    { 1122, "Moss Bros", "mossbros", "moss.co.uk" },
                    { 1123, "Mountain Warehouse", "mountainwarehouse", "mountainwarehouse.com" },
                    { 1124, "Nobody's Child", "nobodyschild", "nobodyschild.com" },
                    { 1125, "Oasis", "oasis", "oasis-stores.com" },
                    { 1126, "Office", "office", "office.co.uk" },
                    { 1127, "Oliver Bonas", "oliverbonas", "oliverbonas.com" },
                    { 1128, "Paul Smith", "paulsmith", "paulsmith.com" },
                    { 1129, "Pavers", "pavers", "pavers.co.uk" },
                    { 1130, "Peacocks", "peacocks", "peacocks.co.uk" },
                    { 1131, "Phase Eight", "phaseeight", "phase-eight.com" },
                    { 1132, "Pull & Bear", "pullbear", "pullandbear.com" },
                    { 1133, "Quiz Clothing", "quizclothing", "quizclothing.co.uk" },
                    { 1134, "Rab", "rab", "rab.equipment" },
                    { 1135, "Radley", "radley", "radley.co.uk" },
                    { 1136, "Ralph Lauren", "ralphlauren", "ralphlauren.co.uk" },
                    { 1137, "Reiss", "reiss", "reiss.com" },
                    { 1138, "Regatta", "regatta", "regatta.com" },
                    { 1139, "Rohan", "rohan", "rohan.co.uk" },
                    { 1140, "Roman Originals", "romanoriginals", "romanoriginals.co.uk" },
                    { 1141, "Sports Shoes", "sportsshoes", "sportsshoes.com" },
                    { 1142, "Schuh", "schuh", "schuh.co.uk" },
                    { 1143, "Seasalt Cornwall", "seasaltcornwall", "seasaltcornwall.com" },
                    { 1144, "Shoe Zone", "shoezone", "shoezone.com" },
                    { 1145, "Simply Be", "simplybe", "simplybe.co.uk" },
                    { 1146, "Size?", "size", "size.co.uk" },
                    { 1147, "Skechers", "skechers", "skechers.co.uk" },
                    { 1148, "Stradivarius", "stradivarius", "stradivarius.com" },
                    { 1149, "Superdry", "superdry", "superdry.com" },
                    { 1150, "Ted Baker", "tedbaker", "tedbaker.com" },
                    { 1151, "The North Face", "thenorthface", "thenorthface.co.uk" },
                    { 1152, "The Outnet", "theoutnet", "theoutnet.com" },
                    { 1153, "Timberland", "timberland", "timberland.co.uk" },
                    { 1154, "Toast", "toast", "toa.st" },
                    { 1155, "Tommy Hilfiger", "tommyhilfiger", "uk.tommy.com" },
                    { 1156, "Topshop", "topshop", "topshop.com" },
                    { 1157, "Trespass", "trespass", "trespass.com" },
                    { 1158, "Tu Clothing", "tuclothing", "sainsburys.co.uk" },
                    { 1159, "Under Armour", "underarmour", "underarmour.co.uk" },
                    { 1160, "Urban Outfitters", "urbanoutfitters", "urbanoutfitters.com" },
                    { 1161, "Vans", "vans", "vans.co.uk" },
                    { 1162, "Very", "very", "very.co.uk" },
                    { 1163, "Vivienne Westwood", "viviennewestwood", "viviennewestwood.com" },
                    { 1164, "Warehouse", "warehouse", "warehousefashion.com" },
                    { 1165, "Weird Fish", "weirdfish", "weirdfish.co.uk" },
                    { 1166, "White Company", "whitecompany", "thewhitecompany.com" },
                    { 1167, "White Stuff", "whitestuff", "whitestuff.com" },
                    { 1168, "Whistles", "whistles", "whistles.com" },
                    { 1169, "Wallis", "wallis", "wallis.co.uk" },
                    { 1170, "Yours Clothing", "yoursclothing", "yoursclothing.co.uk" },
                    { 1171, "Zalando", "zalando", "zalando.co.uk" },
                    { 1172, "Jules B", "julesb", "julesb.co.uk" },
                    { 1173, "End Clothing", "endclothing", "endclothing.com" },
                    { 1174, "Flannels", "flannels", "flannels.com" },
                    { 1175, "Matches Fashion", "matchesfashion", "matchesfashion.com" },
                    { 1176, "Net a Porter", "netaporter", "net-a-porter.com" },
                    { 1177, "Represent", "represent", "representclo.com" },
                    { 1178, "Bakers Plus", "bakersplus", "bakersplus.co.uk" },
                    { 1179, "Banana Tree", "bananatree", "bananatree.co.uk" },
                    { 1180, "Barburrito", "barburrito", "barburrito.co.uk" },
                    { 1181, "Bill's", "bills", "bills-website.co.uk" },
                    { 1182, "Bird", "bird", "birdrestaurants.com" },
                    { 1183, "Black Sheep Coffee", "blacksheepcoffee", "leavetheherd.com" },
                    { 1184, "Boston Tea Party", "bostonteaparty", "bostonteaparty.co.uk" },
                    { 1185, "Boojum", "boojum", "boojummex.com" },
                    { 1186, "Browns Brasserie", "brownsbrasserie", "browns-restaurants.co.uk" },
                    { 1187, "Bubbleology", "bubbleology", "bubbleology.com" },
                    { 1188, "Busaba", "busaba", "busaba.com" },
                    { 1189, "Cafe Rouge", "caferouge", "caferouge.com" },
                    { 1190, "Carluccio's", "carluccios", "carluccios.com" },
                    { 1191, "Chaiiwala", "chaiiwala", "chaiiwala.co.uk" },
                    { 1192, "Chai Point", "chaipoint", "chaipoint.co.uk" },
                    { 1193, "Chiquito", "chiquito", "chiquito.co.uk" },
                    { 1194, "Chopstix", "chopstix", "chopstixgroup.com" },
                    { 1195, "Chozen Noodle", "chozennoodle", "chozennoodle.com" },
                    { 1196, "Ciao Bella", "ciaobella", "ciaobellarestaurant.co.uk" },
                    { 1197, "Cafe Concerto", "cafeconcerto", "cafeconcerto.co" },
                    { 1198, "Coco di Mama", "cocodimama", "cocodimama.co.uk" },
                    { 1199, "Comptoir Libanais", "comptoirlibanais", "comptoirlibanais.com" },
                    { 1200, "Cote Brasserie", "cotebrasserie", "cote.co.uk" },
                    { 1201, "Crussh", "crussh", "crussh.com" },
                    { 1202, "Das Sushi", "dassushi", "dassushi.co.uk" },
                    { 1203, "Dishoom", "dishoom", "dishoom.com" },
                    { 1204, "Ed's Easy Diner", "edseasydiner", "edseasydiner.com" },
                    { 1205, "Eat Tokyo", "eattokyo", "eattokyo.co.uk" },
                    { 1206, "Franco Manca", "francomanca", "francomanca.co.uk" },
                    { 1207, "Frankie & Benny's", "frankiebennys", "frankieandbennys.com" },
                    { 1208, "Gail's Bakery", "gailsbakery", "gailsbread.co.uk" },
                    { 1209, "German Doner Kebab", "germandonerkebab", "germandonerkebab.com" },
                    { 1210, "Giggling Squid", "gigglingsquid", "gigglingsquid.com" },
                    { 1211, "Gourmet Burger Kitchen", "gourmetburgerkitchen", "gbk.co.uk" },
                    { 1212, "Greenwich Market", "greenwichmarket", "greenwichmarket.london" },
                    { 1213, "Hard Rock Cafe", "hardrockcafe", "hardrockcafe.com" },
                    { 1214, "Hawksmoor", "hawksmoor", "thehawksmoor.com" },
                    { 1215, "Honest Burgers", "honestburgers", "honestburgers.co.uk" },
                    { 1216, "Ilford Kebab", "ilfordkebab", "ilfordkebab.co.uk" },
                    { 1217, "Ippudo", "ippudo", "ippudo.co.uk" },
                    { 1218, "Joe & The Juice", "joethejuice", "joejuice.com" },
                    { 1219, "Kaspa's Desserts", "kaspasdesserts", "kaspas.co.uk" },
                    { 1220, "Krispy Kreme", "krispykreme", "krispykreme.co.uk" },
                    { 1221, "La Tasca", "latasca", "latasca.com" },
                    { 1222, "Las Iguanas", "lasiguanas", "iguanas.co.uk" },
                    { 1223, "Loungers", "loungers", "thelounges.co.uk" },
                    { 1224, "Mowgli", "mowgli", "mowglistreetfood.com" },
                    { 1225, "Nudo Sushi", "nudosushi", "nudosushibox.co.uk" },
                    { 1226, "Ole & Steen", "olesteen", "oleandsteen.co.uk" },
                    { 1227, "Pasta Evangelists", "pastaevangelists", "pastaevangelists.com" },
                    { 1228, "Patisserie Valerie", "patisserievalerie", "patisserie-valerie.co.uk" },
                    { 1229, "Paul Bakery", "paulbakery", "paul-uk.com" },
                    { 1230, "Pepe's Piri Piri", "pepespiripiri", "pepes.co.uk" },
                    { 1231, "Pho", "pho", "phocafe.co.uk" },
                    { 1232, "Pizza Pilgrims", "pizzapilgrims", "pizzapilgrims.co.uk" },
                    { 1233, "Pizza Punks", "pizzapunks", "pizzapunks.co.uk" },
                    { 1234, "Popeyes", "popeyes", "popeyes.co.uk" },
                    { 1235, "Rosa's Thai", "rosasthai", "rosasthai.com" },
                    { 1236, "Roti King", "rotiking", "rotiking.com" },
                    { 1237, "Sakura", "sakura", "sakurarestaurant.co.uk" },
                    { 1238, "Shake Shack", "shakeshack", "shakeshack.co.uk" },
                    { 1239, "Slim Chickens", "slimchickens", "slimchickens.co.uk" },
                    { 1240, "Soho Coffee", "sohocoffee", "sohocoffee.co.uk" },
                    { 1241, "Sushi Daily", "sushidaily", "sushidaily.com" },
                    { 1242, "Taco Bell", "tacobell", "tacobell.co.uk" },
                    { 1243, "Tortilla", "tortilla", "tortilla.co.uk" },
                    { 1244, "Turtle Bay", "turtlebay", "turtlebay.co.uk" },
                    { 1245, "Wahaca", "wahaca", "wahaca.co.uk" },
                    { 1246, "Wendy's", "wendys", "wendys.co.uk" },
                    { 1247, "Wingstop", "wingstop", "wingstop.co.uk" },
                    { 1248, "Zambrero", "zambrero", "zambrero.co.uk" },
                    { 1249, "Bella Vista", "bellavista", "bellavista.co.uk" },
                    { 1250, "Big Mamma", "bigmamma", "bigmammagroup.com" },
                    { 1251, "Bunsik", "bunsik", "bunsik.co.uk" },
                    { 1252, "Chick King", "chickking", "chickking.co.uk" },
                    { 1253, "Chicken Cottage", "chickencottage", "chickencottage.com" },
                    { 1254, "Chicken Shop", "chickenshop", "chickenshop.com" },
                    { 1255, "Dixy Chicken", "dixychicken", "dixychicken.co.uk" },
                    { 1256, "Fish and Chips Co", "fishandchipsco", "fishandchips.co.uk" },
                    { 1257, "Harry Ramsden's", "harryramsdens", "harryramsdens.co.uk" },
                    { 1258, "Morley's", "morleys", "morleys.co.uk" },
                    { 1259, "Perfect Fried Chicken", "perfectfriedchicken", "pfcuk.co.uk" },
                    { 1260, "Sam's Chicken", "samschicken", "samschicken.co.uk" },
                    { 1261, "Tossed", "tossed", "tossed.com" },
                    { 1262, "Vital Ingredient", "vitalingredient", "vitalingredient.co.uk" },
                    { 1263, "Abokado", "abokado", "abokado.com" },
                    { 1264, "Benugo", "benugo", "benugo.com" },
                    { 1265, "Caffe Concerto", "caffeconcerto", "caffeconcerto.co.uk" },
                    { 1266, "Coffee Republic", "coffeerepublic", "coffeerepublic.co.uk" },
                    { 1267, "Department of Coffee", "departmentofcoffee", "departmentofcoffee.com" },
                    { 1268, "Grind", "grind", "grind.co.uk" },
                    { 1269, "Harris and Hoole", "harrisandhoole", "harrisandhoole.co.uk" },
                    { 1270, "Notes Coffee", "notescoffee", "notescoffee.com" },
                    { 1271, "Puccino's", "puccinos", "puccinos.com" },
                    { 1272, "Taylor St Baristas", "taylorstbaristas", "taylorstbaristas.com" },
                    { 1273, "Tim Hortons", "timhortons", "timhortons.co.uk" },
                    { 1274, "Bageriet", "bageriet", "bageriet.co.uk" },
                    { 1275, "Bread Ahead", "breadahead", "breadahead.com" },
                    { 1276, "Cinnabon", "cinnabon", "cinnabon.co.uk" },
                    { 1277, "Cookies and Cream", "cookiesandcream", "cookiesandcream.co.uk" },
                    { 1278, "Crosstown Doughnuts", "crosstowndoughnuts", "crosstown.co.uk" },
                    { 1279, "Doughnut Time", "doughnuttime", "doughnuttime.co.uk" },
                    { 1280, "Dum Dums Donutterie", "dumdumsdonutterie", "dumdumsdonutterie.co.uk" },
                    { 1281, "Millie's Cookies", "milliescookies", "milliescookies.com" },
                    { 1282, "Ben's Cookies", "benscookies", "benscookies.com" },
                    { 1283, "Creams Cafe", "creamscafe", "creamscafe.com" },
                    { 1284, "Heavenly Desserts", "heavenlydesserts", "heavenlydesserts.co.uk" },
                    { 1285, "Shakeaway", "shakeaway", "shakeaway.com" },
                    { 1286, "Snowflake Gelato", "snowflakegelato", "snowflakegelato.co.uk" },
                    { 1287, "All Bar One", "allbarone", "allbarone.co.uk" },
                    { 1288, "Be At One", "beatone", "beatone.co.uk" },
                    { 1289, "Brewdog", "brewdog", "brewdog.com" },
                    { 1290, "Brewers Fayre", "brewersfayre", "brewersfayre.co.uk" },
                    { 1291, "Chef & Brewer", "chefbrewer", "chefandbrewer.com" },
                    { 1292, "Craft Union", "craftunion", "craftunionpubs.com" },
                    { 1293, "Ember Inns", "emberinns", "emberinns.co.uk" },
                    { 1294, "Fayre & Square", "fayresquare", "fayre-square.com" },
                    { 1295, "Fullers", "fullers", "fullers.co.uk" },
                    { 1296, "Greene King", "greeneking", "greeneking.co.uk" },
                    { 1297, "Hungry Horse", "hungryhorse", "hungryhorse.co.uk" },
                    { 1298, "Innkeeper's Lodge", "innkeeperslodge", "innkeeperscollection.co.uk" },
                    { 1299, "Marstons", "marstons", "marstons.co.uk" },
                    { 1300, "Mitchells & Butlers", "mitchellsbutlers", "mbplc.com" },
                    { 1301, "Nicholson's Pubs", "nicholsonspubs", "nicholsonspubs.co.uk" },
                    { 1302, "O'Neill's", "oneills", "oneills.co.uk" },
                    { 1303, "Premium Country Pubs", "premiumcountrypubs", "premiumcountrypubs.co.uk" },
                    { 1304, "Revolution Bars", "revolutionbars", "revolutionbars.co.uk" },
                    { 1305, "Sizzling Pubs", "sizzlingpubs", "sizzlingpubs.co.uk" },
                    { 1306, "Stonegate", "stonegate", "stonegategroup.co.uk" },
                    { 1307, "Stonehouse Pizza", "stonehousepizza", "stonehousepizzaandcarvery.co.uk" },
                    { 1308, "Taylor Walker", "taylorwalker", "taylor-walker.co.uk" },
                    { 1309, "The Alchemist", "thealchemist", "thealchemist.uk.com" },
                    { 1310, "Turtle Bay Bar", "turtlebaybar", "turtlebay.co.uk" },
                    { 1311, "Vintage Inns", "vintageinns", "vintageinn.co.uk" },
                    { 1312, "Wetherspoon Hotels", "wetherspoonhotels", "jdwetherspoon.com" },
                    { 1313, "Young's", "youngs", "youngs.co.uk" },
                    { 1314, "Simmons Bar", "simmonsbar", "simmonsbar.co.uk" },
                    { 1315, "Dirty Martini", "dirtymartini", "dirtymartini.uk.com" },
                    { 1316, "Flight Club", "flightclub", "flightclubdarts.com" },
                    { 1317, "Bounce", "bounce", "bouncepingpong.com" },
                    { 1318, "Roxy Ball Room", "roxyballroom", "roxyballroom.co.uk" },
                    { 1319, "Tank & Paddle", "tankpaddle", "tankandpaddle.co.uk" },
                    { 1320, "Brewhouse & Kitchen", "brewhousekitchen", "brewhouseandkitchen.com" },
                    { 1321, "Craft Beer Co", "craftbeerco", "thecraftbeerco.com" },
                    { 1322, "Asda Pharmacy", "asdapharmacy", "asda.com" },
                    { 1323, "Boots Opticians", "bootsopticians", "boots.com" },
                    { 1324, "Cohens Chemist", "cohenschemist", "cohenschemist.co.uk" },
                    { 1325, "Day Lewis Pharmacy", "daylewispharmacy", "daylewis.co.uk" },
                    { 1326, "Rowlands Pharmacy", "rowlandspharmacy", "rowlandspharmacy.co.uk" },
                    { 1327, "Superdrug Pharmacy", "superdrugpharmacy", "superdrug.com" },
                    { 1328, "Tesco Pharmacy", "tescopharmacy", "tesco.com" },
                    { 1329, "Pharmacy2U", "pharmacy2u", "pharmacy2u.co.uk" },
                    { 1330, "Chemist 4 U", "chemist4u", "chemist-4-u.com" },
                    { 1331, "Chemist Direct", "chemistdirect", "chemistdirect.co.uk" },
                    { 1332, "Echo Pharmacy", "echopharmacy", "echo.co.uk" },
                    { 1333, "Lloyds Direct", "lloydsdirect", "lloydsdirect.co.uk" },
                    { 1334, "Weldricks", "weldricks", "weldricks.co.uk" },
                    { 1335, "Boots Hearingcare", "bootshearingcare", "boots.com" },
                    { 1336, "Hidden Hearing", "hiddenhearing", "hiddenhearing.co.uk" },
                    { 1337, "Amplifon", "amplifon", "amplifon.com" },
                    { 1338, "Scrivens", "scrivens", "scrivens.com" },
                    { 1339, "Optical Express", "opticalexpress", "opticalexpress.co.uk" },
                    { 1340, "Leightons Opticians", "leightonsopticians", "leightons.co.uk" },
                    { 1341, "Asda Opticians", "asdaopticians", "asda.com" },
                    { 1342, "Tesco Opticians", "tescoopticians", "tesco.com" },
                    { 1343, "Glasses Direct", "glassesdirect", "glassesdirect.co.uk" },
                    { 1344, "Lenstore", "lenstore", "lenstore.co.uk" },
                    { 1345, "Feel Good Contacts", "feelgoodcontacts", "feelgoodcontacts.com" },
                    { 1346, "Bupa Dental", "bupadental", "bupa.co.uk" },
                    { 1347, "Mydentist", "mydentist", "mydentist.co.uk" },
                    { 1348, "Portman Dental", "portmandental", "portmandentalcare.com" },
                    { 1349, "Rodericks Dental", "rodericksdental", "rodericksdental.co.uk" },
                    { 1350, "Smile Direct Club", "smiledirectclub", "smiledirectclub.co.uk" },
                    { 1351, "Spire Healthcare", "spirehealthcare", "spirehealthcare.com" },
                    { 1352, "Circle Health", "circlehealth", "circlehealthgroup.co.uk" },
                    { 1353, "Ramsay Health Care", "ramsayhealthcare", "ramsayhealth.co.uk" },
                    { 1354, "Nuffield Hospital", "nuffieldhospital", "nuffieldhealth.com" },
                    { 1355, "Babylon Health", "babylonhealth", "babylonhealth.com" },
                    { 1356, "Push Doctor", "pushdoctor", "pushdoctor.co.uk" },
                    { 1357, "Livi", "livi", "livi.co.uk" },
                    { 1358, "Zava", "zava", "zavamed.com" },
                    { 1359, "Superdrug Online Doctor", "superdrugonlinedoctor", "onlinedoctor.superdrug.com" },
                    { 1360, "NHS Prescription", "nhsprescription", "nhs.uk" },
                    { 1361, "Randox Health", "randoxhealth", "randoxhealth.com" },
                    { 1362, "Aveda", "aveda", "aveda.co.uk" },
                    { 1363, "Beauty Bay", "beautybay", "beautybay.com" },
                    { 1364, "Beauty Pie", "beautypie", "beautypie.com" },
                    { 1365, "Benefit Cosmetics", "benefitcosmetics", "benefitcosmetics.com" },
                    { 1366, "Bobbi Brown", "bobbibrown", "bobbibrown.co.uk" },
                    { 1367, "Charlotte Tilbury", "charlottetilbury", "charlottetilbury.com" },
                    { 1368, "Clarins", "clarins", "clarins.co.uk" },
                    { 1369, "Clinique", "clinique", "clinique.co.uk" },
                    { 1370, "Cult Beauty", "cultbeauty", "cultbeauty.co.uk" },
                    { 1371, "Elemis", "elemis", "elemis.com" },
                    { 1372, "Estee Lauder", "esteelauder", "esteelauder.co.uk" },
                    { 1373, "Feelunique", "feelunique", "feelunique.com" },
                    { 1374, "Glossier", "glossier", "glossier.com" },
                    { 1375, "Kiehl's", "kiehls", "kiehls.co.uk" },
                    { 1376, "L'Occitane", "loccitane", "loccitane.com" },
                    { 1377, "Look Fantastic", "lookfantastic", "lookfantastic.com" },
                    { 1378, "MAC Cosmetics", "maccosmetics", "maccosmetics.co.uk" },
                    { 1379, "Molton Brown", "moltonbrown", "moltonbrown.co.uk" },
                    { 1380, "Neal's Yard Remedies", "nealsyardremedies", "nealsyardremedies.com" },
                    { 1381, "Origins", "origins", "origins.co.uk" },
                    { 1382, "Rituals", "rituals", "rituals.com" },
                    { 1383, "Sanctuary Spa", "sanctuaryspa", "sanctuary.com" },
                    { 1384, "Sculpted by Aimee", "sculptedbyaimee", "sculptedbyaimee.com" },
                    { 1385, "Skinstore", "skinstore", "skinstore.com" },
                    { 1386, "The Ordinary", "theordinary", "theordinary.com" },
                    { 1387, "The Perfume Shop", "theperfumeshop", "theperfumeshop.com" },
                    { 1388, "Toni & Guy", "toniguy", "toniandguy.com" },
                    { 1389, "Jo Malone", "jomalone", "jomalone.co.uk" },
                    { 1390, "Penhaligon's", "penhaligons", "penhaligons.com" },
                    { 1391, "Bleach London", "bleachlondon", "bleachlondon.com" },
                    { 1392, "Regis Salon", "regissalon", "regissalons.co.uk" },
                    { 1393, "Supercuts", "supercuts", "supercuts.co.uk" },
                    { 1394, "Nails Inc", "nailsinc", "nailsinc.com" },
                    { 1395, "Sweaty Betty Beauty", "sweatybettybeauty", "sweatybetty.com" },
                    { 1396, "Fragrance Direct", "fragrancedirect", "fragrancedirect.co.uk" },
                    { 1397, "Argos Tech", "argostech", "argos.co.uk" },
                    { 1398, "BT Shop", "btshop", "btshop.bt.com" },
                    { 1399, "CeX", "cex", "webuy.com" },
                    { 1400, "Cash Converters", "cashconverters", "cashconverters.co.uk" },
                    { 1401, "Dell", "dell", "dell.co.uk" },
                    { 1402, "Ebuyer", "ebuyer", "ebuyer.com" },
                    { 1403, "Elekdirect", "elekdirect", "elekdirect.co.uk" },
                    { 1404, "Google Store", "googlestore", "store.google.com" },
                    { 1405, "HP Store", "hpstore", "hp.com" },
                    { 1406, "Hughes", "hughes", "hughes.co.uk" },
                    { 1407, "Insight UK", "insightuk", "uk.insight.com" },
                    { 1408, "John Lewis Tech", "johnlewistech", "johnlewis.com" },
                    { 1409, "Laptops Direct", "laptopsdirect", "laptopsdirect.co.uk" },
                    { 1410, "Lenovo", "lenovo", "lenovo.com" },
                    { 1411, "Maplin", "maplin", "maplin.co.uk" },
                    { 1412, "Marks Electrical", "markselectrical", "markselectrical.co.uk" },
                    { 1413, "Novatech", "novatech", "novatech.co.uk" },
                    { 1414, "Overclockers UK", "overclockersuk", "overclockers.co.uk" },
                    { 1415, "PC Specialist", "pcspecialist", "pcspecialist.co.uk" },
                    { 1416, "Richer Sounds", "richersounds", "richersounds.com" },
                    { 1417, "Scan Computers", "scancomputers", "scan.co.uk" },
                    { 1418, "Sonos", "sonos", "sonos.com" },
                    { 1419, "Sony", "sony", "sony.co.uk" },
                    { 1420, "Very Tech", "verytech", "very.co.uk" },
                    { 1421, "Box", "box", "box.co.uk" },
                    { 1422, "Amazon Devices", "amazondevices", "amazon.co.uk" },
                    { 1423, "Bose", "bose", "bose.co.uk" },
                    { 1424, "Dyson", "dyson", "dyson.co.uk" },
                    { 1425, "GAME Digital", "gamedigital", "game.co.uk" },
                    { 1426, "Nintendo Store", "nintendostore", "nintendo.co.uk" },
                    { 1427, "PlayStation Store", "playstationstore", "playstation.com" },
                    { 1428, "Xbox Store", "xboxstore", "xbox.com" },
                    { 1429, "Steam", "steam", "steampowered.com" },
                    { 1430, "Epic Games", "epicgames", "epicgames.com" },
                    { 1431, "Music Magpie", "musicmagpie", "musicmagpie.co.uk" },
                    { 1432, "Anglian Home", "anglianhome", "anglianhome.co.uk" },
                    { 1433, "Bensons for Beds", "bensonsforbeds", "bensonsforbeds.co.uk" },
                    { 1434, "Bathstore", "bathstore", "bathstore.com" },
                    { 1435, "Bathroom Village", "bathroomvillage", "bathroomvillage.com" },
                    { 1436, "Betta Living", "bettaliving", "bettaliving.co.uk" },
                    { 1437, "Buildbase", "buildbase", "buildbase.co.uk" },
                    { 1438, "Carpetright", "carpetright", "carpetright.co.uk" },
                    { 1439, "City Plumbing", "cityplumbing", "cityplumbing.co.uk" },
                    { 1440, "Cotswold Company", "cotswoldcompany", "cotswoldco.com" },
                    { 1441, "Cox & Cox", "coxcox", "coxandcox.co.uk" },
                    { 1442, "Dreams", "dreams", "dreams.co.uk" },
                    { 1443, "Dulux Decorator Centre", "duluxdecoratorcentre", "duluxdecoratorcentre.co.uk" },
                    { 1444, "Fired Earth", "firedearth", "firedearth.com" },
                    { 1445, "Furniture Village", "furniturevillage", "furniturevillage.co.uk" },
                    { 1446, "Graham Plumbers Merchant", "grahamplumbersmerchant", "grahamplumbersmerchant.co.uk" },
                    { 1447, "Habitat", "habitat", "habitat.co.uk" },
                    { 1448, "Harveys Furniture", "harveysfurniture", "harveysfurniture.co.uk" },
                    { 1449, "Homesense", "homesense", "homesense.com" },
                    { 1450, "Howdens", "howdens", "howdens.com" },
                    { 1451, "Hillarys", "hillarys", "hillarys.co.uk" },
                    { 1452, "Julian Charles", "juliancharles", "juliancharles.co.uk" },
                    { 1453, "Laura Ashley", "lauraashley", "lauraashley.com" },
                    { 1454, "Loaf", "loaf", "loaf.com" },
                    { 1455, "Magnet", "magnet", "magnet.co.uk" },
                    { 1456, "Maisons du Monde", "maisonsdumonde", "maisonsdumonde.com" },
                    { 1457, "MFI", "mfi", "mfi.co.uk" },
                    { 1458, "Neptune", "neptune", "neptune.com" },
                    { 1459, "Next Home", "nexthome", "next.co.uk" },
                    { 1460, "Oak Furnitureland", "oakfurnitureland", "oakfurnitureland.co.uk" },
                    { 1461, "Plumbworld", "plumbworld", "plumbworld.co.uk" },
                    { 1462, "Selco", "selco", "selcobw.com" },
                    { 1463, "ScS", "scs", "scs.co.uk" },
                    { 1464, "Sharps Bedrooms", "sharpsbedrooms", "sharps.co.uk" },
                    { 1465, "Sofology", "sofology", "sofology.co.uk" },
                    { 1466, "Soak", "soak", "soak.com" },
                    { 1467, "Swoon", "swoon", "swooneditions.com" },
                    { 1468, "Tile Giant", "tilegiant", "tilegiant.co.uk" },
                    { 1469, "Tile Mountain", "tilemountain", "tilemountain.co.uk" },
                    { 1470, "Topps Tiles", "toppstiles", "toppstiles.co.uk" },
                    { 1471, "Victoria Plum", "victoriaplum", "victoriaplum.com" },
                    { 1472, "Wren Kitchens", "wrenkitchens", "wrenkitchens.com" },
                    { 1473, "Brewers Decorator", "brewersdecorator", "brewers.co.uk" },
                    { 1474, "Crown Paints", "crownpaints", "crownpaints.co.uk" },
                    { 1475, "Farrow & Ball", "farrowball", "farrow-ball.com" },
                    { 1476, "Leyland SDM", "leylandsdm", "leylandsdm.co.uk" },
                    { 1477, "Machine Mart", "machinemart", "machinemart.co.uk" },
                    { 1478, "Axminster Tools", "axminstertools", "axminstertools.com" },
                    { 1479, "ITS", "its", "its.co.uk" },
                    { 1480, "Ironmongery Direct", "ironmongerydirect", "ironmongerydirect.co.uk" },
                    { 1481, "Trade Point", "tradepoint", "trade-point.co.uk" },
                    { 1482, "The Original Factory Shop", "theoriginalfactoryshop", "tofs.com" },
                    { 1483, "Matalan", "matalan", "matalan.co.uk" },
                    { 1484, "QD Stores", "qdstores", "qdstores.co.uk" },
                    { 1485, "Boyes", "boyes", "boyes.co.uk" },
                    { 1486, "Blue Diamond Garden", "bluediamondgarden", "bluediamond.gg" },
                    { 1487, "British Garden Centres", "britishgardencentres", "britishgardencentres.com" },
                    { 1488, "Crocus", "crocus", "crocus.co.uk" },
                    { 1489, "Gardening Direct", "gardeningdirect", "gardeningdirect.co.uk" },
                    { 1490, "Hillier Garden Centres", "hilliergardencentres", "hillier.co.uk" },
                    { 1491, "Notcutts", "notcutts", "notcutts.co.uk" },
                    { 1492, "Squire's Garden", "squiresgarden", "squiresgardencentres.co.uk" },
                    { 1493, "Suttons Seeds", "suttonsseeds", "suttons.co.uk" },
                    { 1494, "Thompson & Morgan", "thompsonmorgan", "thompson-morgan.com" },
                    { 1495, "Wyevale", "wyevale", "wyevalegardencentres.co.uk" },
                    { 1496, "Homebase Garden", "homebasegarden", "homebase.co.uk" },
                    { 1497, "Gardman", "gardman", "gardman.co.uk" },
                    { 1498, "Pets at Home", "petsathome", "petsathome.com" },
                    { 1499, "Jollyes", "jollyes", "jollyes.co.uk" },
                    { 1500, "Pets Corner", "petscorner", "petscorner.co.uk" },
                    { 1501, "Zooplus", "zooplus", "zooplus.co.uk" },
                    { 1502, "Fetch", "fetch", "fetch.co.uk" },
                    { 1503, "Bern Pet Foods", "bernpetfoods", "bernpetfoods.co.uk" },
                    { 1504, "Vets4Pets", "vets4pets", "vets4pets.com" },
                    { 1505, "Medivet", "medivet", "medivetgroup.com" },
                    { 1506, "Companion Care", "companioncare", "companioncare.co.uk" },
                    { 1507, "Blue Cross", "bluecross", "bluecross.org.uk" },
                    { 1508, "PDSA", "pdsa", "pdsa.org.uk" },
                    { 1509, "Tails.com", "tailscom", "tails.com" },
                    { 1510, "Butternut Box", "butternutbox", "butternutbox.com" },
                    { 1511, "Waterstones", "waterstones", "waterstones.com" },
                    { 1512, "Blackwell's", "blackwells", "blackwells.co.uk" },
                    { 1513, "Foyles", "foyles", "foyles.co.uk" },
                    { 1514, "WHSmith", "whsmith", "whsmith.co.uk" },
                    { 1515, "The Works", "theworks", "theworks.co.uk" },
                    { 1516, "Wordery", "wordery", "wordery.com" },
                    { 1517, "World of Books", "worldofbooks", "wob.com" },
                    { 1518, "Book Depository", "bookdepository", "bookdepository.com" },
                    { 1519, "Hive", "hive", "hive.co.uk" },
                    { 1520, "Barnes & Noble", "barnesnoble", "barnesandnoble.com" },
                    { 1521, "Ryman", "ryman", "ryman.co.uk" },
                    { 1522, "Paperchase", "paperchase", "paperchase.com" },
                    { 1523, "Cass Art", "cassart", "cassart.co.uk" },
                    { 1524, "Hobbycraft", "hobbycraft", "hobbycraft.co.uk" },
                    { 1525, "The Range Crafts", "therangecrafts", "therange.co.uk" },
                    { 1526, "Staples", "staples", "staples.co.uk" },
                    { 1527, "Viking Direct", "vikingdirect", "viking-direct.co.uk" },
                    { 1528, "Office Outlet", "officeoutlet", "officeoutlet.com" },
                    { 1529, "Euroffice", "euroffice", "euroffice.co.uk" },
                    { 1530, "The Entertainer", "theentertainer", "thetoyshop.com" },
                    { 1531, "Hamleys", "hamleys", "hamleys.com" },
                    { 1532, "Lego Store", "legostore", "lego.com" },
                    { 1533, "Build-A-Bear", "buildabear", "buildabear.co.uk" },
                    { 1534, "Character.com", "charactercom", "character.com" },
                    { 1535, "Toymaster", "toymaster", "toymaster.co.uk" },
                    { 1536, "Games Workshop", "gamesworkshop", "games-workshop.com" },
                    { 1537, "Warhammer", "warhammer", "warhammer.com" },
                    { 1538, "Card Factory", "cardfactory", "cardfactory.co.uk" },
                    { 1539, "Clintons", "clintons", "clintonsretail.com" },
                    { 1540, "Moonpig", "moonpig", "moonpig.com" },
                    { 1541, "Funky Pigeon", "funkypigeon", "funkypigeon.com" },
                    { 1542, "Not on the High Street", "notonthehighstreet", "notonthehighstreet.com" },
                    { 1543, "Etsy", "etsy", "etsy.com" },
                    { 1544, "Prezzybox", "prezzybox", "prezzybox.com" },
                    { 1545, "Menkind", "menkind", "menkind.co.uk" },
                    { 1546, "Firebox", "firebox", "firebox.com" },
                    { 1547, "Interflora", "interflora", "interflora.co.uk" },
                    { 1548, "Bloom & Wild", "bloomwild", "bloomandwild.com" },
                    { 1549, "Serenata Flowers", "serenataflowers", "serenataflowers.com" },
                    { 1550, "Hotel Chocolat", "hotelchocolat", "hotelchocolat.com" },
                    { 1551, "Wiggle", "wiggle", "wiggle.co.uk" },
                    { 1552, "Chain Reaction Cycles", "chainreactioncycles", "chainreactioncycles.com" },
                    { 1553, "Evans Cycles", "evanscycles", "evanscycles.com" },
                    { 1554, "Halfords Cycling", "halfordscycling", "halfords.com" },
                    { 1555, "Tredz", "tredz", "tredz.co.uk" },
                    { 1556, "Rutland Cycling", "rutlandcycling", "rutlandcycling.com" },
                    { 1557, "Cycle Republic", "cyclerepublic", "cyclerepublic.com" },
                    { 1558, "Sigma Sports", "sigmasports", "sigmasports.com" },
                    { 1559, "Go Outdoors", "gooutdoors", "gooutdoors.co.uk" },
                    { 1560, "Millets", "millets", "millets.co.uk" },
                    { 1561, "Snow and Rock", "snowandrock", "snowandrock.com" },
                    { 1562, "Ellis Brigham", "ellisbrigham", "ellis-brigham.com" },
                    { 1563, "Runners Need", "runnersneed", "runnersneed.com" },
                    { 1564, "Sweatshop", "sweatshop", "sweatshop.co.uk" },
                    { 1565, "Up and Running", "upandrunning", "upandrunning.co.uk" },
                    { 1566, "Wiggle Running", "wigglerunning", "wiggle.co.uk" },
                    { 1567, "Sweaty Betty", "sweatybetty", "sweatybetty.com" },
                    { 1568, "Castore", "castore", "castore.com" },
                    { 1569, "Lovell Rugby", "lovellrugby", "lovell-rugby.co.uk" },
                    { 1570, "Rugby Store", "rugbystore", "rugbystore.co.uk" },
                    { 1571, "Pro Direct Soccer", "prodirectsoccer", "prodirectsoccer.com" },
                    { 1572, "Kitbag", "kitbag", "kitbag.com" },
                    { 1573, "Golf Online", "golfonline", "golfonline.co.uk" },
                    { 1574, "American Golf", "americangolf", "americangolf.co.uk" },
                    { 1575, "Direct Golf", "directgolf", "directgolf.co.uk" },
                    { 1576, "Fishing Republic", "fishingrepublic", "fishingrepublic.net" },
                    { 1577, "Angling Direct", "anglingdirect", "anglingdirect.co.uk" },
                    { 1578, "Surfdome", "surfdome", "surfdome.com" },
                    { 1579, "Boardriders", "boardriders", "boardriders.co.uk" },
                    { 1580, "Absolute Snow", "absolutesnow", "absolute-snow.co.uk" },
                    { 1581, "Decathlon Outdoor", "decathlonoutdoor", "decathlon.co.uk" },
                    { 1582, "Alpkit", "alpkit", "alpkit.com" },
                    { 1583, "Tiso", "tiso", "tiso.com" },
                    { 1584, "Nevisport", "nevisport", "nevisport.com" },
                    { 1585, "Simply Hike", "simplyhike", "simplyhike.co.uk" },
                    { 1586, "Ernest Jones", "ernestjones", "ernestjones.co.uk" },
                    { 1587, "H Samuel", "hsamuel", "hsamuel.co.uk" },
                    { 1588, "Beaverbrooks", "beaverbrooks", "beaverbrooks.co.uk" },
                    { 1589, "Goldsmiths", "goldsmiths", "goldsmiths.co.uk" },
                    { 1590, "Fraser Hart", "fraserhart", "fraserhart.co.uk" },
                    { 1591, "Warren James", "warrenjames", "warrenjames.co.uk" },
                    { 1592, "Pandora", "pandora", "pandora.net" },
                    { 1593, "Swarovski", "swarovski", "swarovski.com" },
                    { 1594, "Thomas Sabo", "thomassabo", "thomassabo.com" },
                    { 1595, "Links of London", "linksoflondon", "linksoflondon.com" },
                    { 1596, "Monica Vinader", "monicavinader", "monicavinader.com" },
                    { 1597, "Astley Clarke", "astleyclarke", "astleyclarke.com" },
                    { 1598, "Missoma", "missoma", "missoma.com" },
                    { 1599, "Accessorize", "accessorize", "accessorize.com" },
                    { 1600, "Claire's", "claires", "claires.com" },
                    { 1601, "Watch Shop", "watchshop", "watchshop.com" },
                    { 1602, "Watches of Switzerland", "watchesofswitzerland", "watches-of-switzerland.co.uk" },
                    { 1603, "Tag Heuer", "tagheuer", "tagheuer.com" },
                    { 1604, "Rolex", "rolex", "rolex.com" },
                    { 1605, "Fossil", "fossil", "fossil.com" },
                    { 1606, "Daniel Wellington", "danielwellington", "danielwellington.com" },
                    { 1607, "Olivia Burton", "oliviaburton", "oliviaburton.com" },
                    { 1608, "Michael Kors", "michaelkors", "michaelkors.co.uk" },
                    { 1609, "Mulberry", "mulberry", "mulberry.com" },
                    { 1610, "Cath Kidston", "cathkidston", "cathkidston.com" },
                    { 1611, "Barclays", "barclays", "barclays.co.uk" },
                    { 1612, "HSBC", "hsbc", "hsbc.co.uk" },
                    { 1613, "Lloyds Bank", "lloydsbank", "lloydsbank.com" },
                    { 1614, "NatWest", "natwest", "natwest.com" },
                    { 1615, "Santander", "santander", "santander.co.uk" },
                    { 1616, "Halifax", "halifax", "halifax.co.uk" },
                    { 1617, "Nationwide", "nationwide", "nationwide.co.uk" },
                    { 1618, "TSB", "tsb", "tsb.co.uk" },
                    { 1619, "Royal Bank of Scotland", "royalbankofscotland", "rbs.co.uk" },
                    { 1620, "Bank of Scotland", "bankofscotland", "bankofscotland.co.uk" },
                    { 1621, "Co-operative Bank", "cooperativebank", "co-operativebank.co.uk" },
                    { 1622, "Metro Bank", "metrobank", "metrobankonline.co.uk" },
                    { 1623, "Monzo", "monzo", "monzo.com" },
                    { 1624, "Starling Bank", "starlingbank", "starlingbank.com" },
                    { 1625, "Revolut", "revolut", "revolut.com" },
                    { 1626, "Chase UK", "chaseuk", "chase.co.uk" },
                    { 1627, "First Direct", "firstdirect", "firstdirect.com" },
                    { 1628, "Virgin Money", "virginmoney", "virginmoney.com" },
                    { 1629, "Yorkshire Building Society", "yorkshirebuildingsociety", "ybs.co.uk" },
                    { 1630, "Skipton Building Society", "skiptonbuildingsociety", "skipton.co.uk" },
                    { 1631, "Coventry Building Society", "coventrybuildingsociety", "coventrybuildingsociety.co.uk" },
                    { 1632, "Aviva", "aviva", "aviva.co.uk" },
                    { 1633, "Direct Line", "directline", "directline.com" },
                    { 1634, "Admiral", "admiral", "admiral.com" },
                    { 1635, "Churchill", "churchill", "churchill.com" },
                    { 1636, "Hastings Direct", "hastingsdirect", "hastingsdirect.com" },
                    { 1637, "LV", "lv", "lv.com" },
                    { 1638, "More Than", "morethan", "morethan.com" },
                    { 1639, "Esure", "esure", "esure.com" },
                    { 1640, "Legal & General", "legalgeneral", "legalandgeneral.com" },
                    { 1641, "Scottish Widows", "scottishwidows", "scottishwidows.co.uk" },
                    { 1642, "Zurich", "zurich", "zurich.co.uk" },
                    { 1643, "AXA", "axa", "axa.co.uk" },
                    { 1644, "Allianz", "allianz", "allianz.co.uk" },
                    { 1645, "Compare the Market", "comparethemarket", "comparethemarket.com" },
                    { 1646, "MoneySuperMarket", "moneysupermarket", "moneysupermarket.com" },
                    { 1647, "GoCompare", "gocompare", "gocompare.com" },
                    { 1648, "Confused.com", "confusedcom", "confused.com" },
                    { 1649, "Hargreaves Lansdown", "hargreaveslansdown", "hl.co.uk" },
                    { 1650, "AJ Bell", "ajbell", "ajbell.co.uk" },
                    { 1651, "Vanguard UK", "vanguarduk", "vanguardinvestor.co.uk" },
                    { 1652, "Nutmeg", "nutmeg", "nutmeg.com" },
                    { 1653, "Wise", "wise", "wise.com" },
                    { 1654, "American Express", "americanexpress", "americanexpress.com" },
                    { 1655, "Capital One", "capitalone", "capitalone.co.uk" },
                    { 1656, "Sky Mobile", "skymobile", "sky.com" },
                    { 1657, "Virgin Mobile", "virginmobile", "virginmedia.com" },
                    { 1658, "Lebara", "lebara", "lebara.co.uk" },
                    { 1659, "Lycamobile", "lycamobile", "lycamobile.co.uk" },
                    { 1660, "VOXI", "voxi", "voxi.co.uk" },
                    { 1661, "SMARTY", "smarty", "smarty.co.uk" },
                    { 1662, "iD Mobile", "idmobile", "idmobile.co.uk" },
                    { 1663, "Shell Energy", "shellenergy", "shellenergy.co.uk" },
                    { 1664, "Utility Warehouse", "utilitywarehouse", "utilitywarehouse.co.uk" },
                    { 1665, "Bulb", "bulb", "bulb.co.uk" },
                    { 1666, "So Energy", "soenergy", "so.energy" },
                    { 1667, "Good Energy", "goodenergy", "goodenergy.co.uk" },
                    { 1668, "Ecotricity", "ecotricity", "ecotricity.co.uk" },
                    { 1669, "Utilita", "utilita", "utilita.co.uk" },
                    { 1670, "Anglian Water", "anglianwater", "anglianwater.co.uk" },
                    { 1671, "Yorkshire Water", "yorkshirewater", "yorkshirewater.com" },
                    { 1672, "United Utilities", "unitedutilities", "unitedutilities.com" },
                    { 1673, "Southern Water", "southernwater", "southernwater.co.uk" },
                    { 1674, "Wessex Water", "wessexwater", "wessexwater.co.uk" },
                    { 1675, "South West Water", "southwestwater", "southwestwater.co.uk" },
                    { 1676, "Scottish Water", "scottishwater", "scottishwater.co.uk" },
                    { 1677, "Northumbrian Water", "northumbrianwater", "nwl.co.uk" },
                    { 1678, "Council Tax", "counciltax", "gov.uk" },
                    { 1679, "HMRC", "hmrc", "gov.uk" },
                    { 1680, "Royal Mail", "royalmail", "royalmail.com" },
                    { 1681, "Premier Inn", "premierinn", "premierinn.com" },
                    { 1682, "Travelodge", "travelodge", "travelodge.co.uk" },
                    { 1683, "Holiday Inn", "holidayinn", "ihg.com" },
                    { 1684, "Hilton", "hilton", "hilton.com" },
                    { 1685, "Marriott", "marriott", "marriott.co.uk" },
                    { 1686, "Ibis", "ibis", "accor.com" },
                    { 1687, "Novotel", "novotel", "accor.com" },
                    { 1688, "Mercure", "mercure", "accor.com" },
                    { 1689, "Radisson Blu", "radissonblu", "radissonhotels.com" },
                    { 1690, "Village Hotels", "villagehotels", "village-hotels.co.uk" },
                    { 1691, "Malmaison", "malmaison", "malmaison.com" },
                    { 1692, "Hotel du Vin", "hotelduvin", "hotelduvin.com" },
                    { 1693, "YHA", "yha", "yha.org.uk" },
                    { 1694, "Center Parcs", "centerparcs", "centerparcs.co.uk" },
                    { 1695, "Butlins", "butlins", "butlins.com" },
                    { 1696, "Haven", "haven", "haven.com" },
                    { 1697, "Pontins", "pontins", "pontins.com" },
                    { 1698, "Booking.com", "bookingcom", "booking.com" },
                    { 1699, "Expedia", "expedia", "expedia.co.uk" },
                    { 1700, "Hotels.com", "hotelscom", "hotels.com" },
                    { 1701, "Airbnb", "airbnb", "airbnb.co.uk" },
                    { 1702, "Trivago", "trivago", "trivago.co.uk" },
                    { 1703, "Skyscanner", "skyscanner", "skyscanner.net" },
                    { 1704, "Lastminute.com", "lastminutecom", "lastminute.com" },
                    { 1705, "On the Beach", "onthebeach", "onthebeach.co.uk" },
                    { 1706, "Loveholidays", "loveholidays", "loveholidays.com" },
                    { 1707, "Jet2holidays", "jet2holidays", "jet2holidays.com" },
                    { 1708, "First Choice", "firstchoice", "firstchoice.co.uk" },
                    { 1709, "Thomas Cook", "thomascook", "thomascook.com" },
                    { 1710, "Virgin Atlantic", "virginatlantic", "virginatlantic.com" },
                    { 1711, "Wizz Air", "wizzair", "wizzair.com" },
                    { 1712, "Vueling", "vueling", "vueling.com" },
                    { 1713, "Aer Lingus", "aerlingus", "aerlingus.com" },
                    { 1714, "KLM", "klm", "klm.co.uk" },
                    { 1715, "Emirates", "emirates", "emirates.com" },
                    { 1716, "P&O Ferries", "poferries", "poferries.com" },
                    { 1717, "DFDS", "dfds", "dfds.co.uk" },
                    { 1718, "Brittany Ferries", "brittanyferries", "brittanyferries.co.uk" },
                    { 1719, "Stena Line", "stenaline", "stenaline.co.uk" },
                    { 1720, "LeShuttle", "leshuttle", "leshuttle.com" },
                    { 1721, "Enterprise Rent-A-Car", "enterpriserentacar", "enterprise.co.uk" },
                    { 1722, "Hertz", "hertz", "hertz.co.uk" },
                    { 1723, "Avis", "avis", "avis.co.uk" },
                    { 1724, "Europcar", "europcar", "europcar.co.uk" },
                    { 1725, "Sixt", "sixt", "sixt.co.uk" },
                    { 1726, "Zipcar", "zipcar", "zipcar.co.uk" },
                    { 1727, "NCP Parking", "ncpparking", "ncp.co.uk" },
                    { 1728, "RingGo", "ringgo", "ringgo.co.uk" },
                    { 1729, "JustPark", "justpark", "justpark.com" },
                    { 1730, "APCOA Parking", "apcoaparking", "apcoa.co.uk" },
                    { 1731, "Odeon", "odeon", "odeon.co.uk" },
                    { 1732, "Cineworld", "cineworld", "cineworld.co.uk" },
                    { 1733, "Vue Cinemas", "vuecinemas", "myvue.com" },
                    { 1734, "Everyman Cinema", "everymancinema", "everymancinema.com" },
                    { 1735, "Picturehouse", "picturehouse", "picturehouses.com" },
                    { 1736, "Showcase Cinemas", "showcasecinemas", "showcasecinemas.co.uk" },
                    { 1737, "Curzon", "curzon", "curzon.com" },
                    { 1738, "Ticketmaster", "ticketmaster", "ticketmaster.co.uk" },
                    { 1739, "See Tickets", "seetickets", "seetickets.com" },
                    { 1740, "ATG Tickets", "atgtickets", "atgtickets.com" },
                    { 1741, "Eventbrite", "eventbrite", "eventbrite.co.uk" },
                    { 1742, "Merlin Entertainments", "merlinentertainments", "merlinentertainments.biz" },
                    { 1743, "Alton Towers", "altontowers", "altontowers.com" },
                    { 1744, "Thorpe Park", "thorpepark", "thorpepark.com" },
                    { 1745, "Legoland Windsor", "legolandwindsor", "legoland.co.uk" },
                    { 1746, "Chessington", "chessington", "chessington.com" },
                    { 1747, "Madame Tussauds", "madametussauds", "madametussauds.com" },
                    { 1748, "London Eye", "londoneye", "londoneye.com" },
                    { 1749, "Sea Life", "sealife", "visitsealife.com" },
                    { 1750, "National Trust", "nationaltrust", "nationaltrust.org.uk" },
                    { 1751, "English Heritage", "englishheritage", "english-heritage.org.uk" },
                    { 1752, "Historic Royal Palaces", "historicroyalpalaces", "hrp.org.uk" },
                    { 1753, "ZSL London Zoo", "zsllondonzoo", "zsl.org" },
                    { 1754, "Hollywood Bowl", "hollywoodbowl", "hollywoodbowl.co.uk" },
                    { 1755, "Tenpin", "tenpin", "tenpin.co.uk" },
                    { 1756, "Nuffield Health Gym", "nuffieldhealthgym", "nuffieldhealth.com" },
                    { 1757, "Anytime Fitness", "anytimefitness", "anytimefitness.co.uk" },
                    { 1758, "Fitness First", "fitnessfirst", "fitnessfirst.co.uk" },
                    { 1759, "Bannatyne", "bannatyne", "bannatyne.co.uk" },
                    { 1760, "Better Leisure", "betterleisure", "better.org.uk" },
                    { 1761, "Vinted", "vinted", "vinted.co.uk" },
                    { 1762, "Depop", "depop", "depop.com" },
                    { 1763, "Gumtree", "gumtree", "gumtree.com" },
                    { 1764, "Facebook Marketplace", "facebookmarketplace", "facebook.com" },
                    { 1765, "OnBuy", "onbuy", "onbuy.com" },
                    { 1766, "Wish", "wish", "wish.com" },
                    { 1767, "AliExpress", "aliexpress", "aliexpress.com" },
                    { 1768, "Temu", "temu", "temu.com" },
                    { 1769, "Groupon", "groupon", "groupon.co.uk" },
                    { 1770, "Wowcher", "wowcher", "wowcher.co.uk" },
                    { 1771, "Trustpilot", "trustpilot", "trustpilot.com" },
                    { 1772, "Just Park Services", "justparkservices", "justpark.com" },
                    { 1773, "Checkatrade", "checkatrade", "checkatrade.com" },
                    { 1774, "Rated People", "ratedpeople", "ratedpeople.com" },
                    { 1775, "MyBuilder", "mybuilder", "mybuilder.com" },
                    { 1776, "TaskRabbit", "taskrabbit", "taskrabbit.co.uk" },
                    { 1777, "Fiverr", "fiverr", "fiverr.com" },
                    { 1778, "Upwork", "upwork", "upwork.com" },
                    { 1779, "Etsy UK", "etsyuk", "etsy.com" },
                    { 1780, "Redbubble", "redbubble", "redbubble.com" },
                    { 1781, "Vistaprint", "vistaprint", "vistaprint.co.uk" },
                    { 1782, "Moo", "moo", "moo.com" },
                    { 1783, "Snappy Snaps", "snappysnaps", "snappysnaps.co.uk" },
                    { 1784, "Photobox", "photobox", "photobox.co.uk" },
                    { 1785, "Shutterstock", "shutterstock", "shutterstock.com" },
                    { 1786, "Canva", "canva", "canva.com" },
                    { 1787, "Squarespace", "squarespace", "squarespace.com" },
                    { 1788, "Wix", "wix", "wix.com" },
                    { 1789, "GoDaddy", "godaddy", "godaddy.com" },
                    { 1790, "123 Reg", "123reg", "123-reg.co.uk" },
                    { 1791, "Ionos", "ionos", "ionos.co.uk" },
                    { 1792, "OVH", "ovh", "ovhcloud.com" },
                    { 1793, "Hetzner", "hetzner", "hetzner.com" },
                    { 1794, "DigitalOcean", "digitalocean", "digitalocean.com" },
                    { 1795, "Linode", "linode", "linode.com" },
                    { 1796, "Evri", "evri", "evri.com" },
                    { 1797, "DPD", "dpd", "dpd.co.uk" },
                    { 1798, "Yodel", "yodel", "yodel.co.uk" },
                    { 1799, "ParcelForce", "parcelforce", "parcelforce.com" },
                    { 1800, "UPS", "ups", "ups.com" }
                });

            migrationBuilder.Sql(RepointExpensesToSeededMerchant);
            migrationBuilder.Sql(RepointAliasesToSeededMerchant);
            migrationBuilder.Sql(DeleteMergedDuplicates);

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_NormalizedName",
                table: "Merchants",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.Sql(ResetMerchantSequences);
        }

        /// <summary>
        /// Seeded ids live in 1-200 and 1001-1800; 201-1000 is left for merchants users create at
        /// runtime. Where a seeded row now duplicates one of those, the seeded row wins — it is the
        /// one carrying a real website — and the user-created row is folded into it.
        /// </summary>
        private const string DuplicateCte = """
            WITH dupes AS (
                SELECT "NormalizedName",
                       MIN("Id") FILTER (WHERE "Id" <= 200 OR "Id" >= 1001) AS keep_id
                FROM "Merchants"
                GROUP BY "NormalizedName"
                HAVING COUNT(*) > 1
            ),
            losers AS (
                SELECT m."Id" AS old_id, d.keep_id
                FROM "Merchants" m
                JOIN dupes d ON d."NormalizedName" = m."NormalizedName"
                WHERE d.keep_id IS NOT NULL AND m."Id" <> d.keep_id
            )

            """;

        private const string RepointExpensesToSeededMerchant = DuplicateCte + """
            UPDATE "Expenses" e
            SET "MerchantId" = l.keep_id
            FROM losers l
            WHERE e."MerchantId" = l.old_id;
            """;

        private const string RepointAliasesToSeededMerchant = DuplicateCte + """
            UPDATE "MerchantAliases" a
            SET "MerchantId" = l.keep_id
            FROM losers l
            WHERE a."MerchantId" = l.old_id;
            """;

        private const string DeleteMergedDuplicates = DuplicateCte + """
            DELETE FROM "Merchants" m
            USING losers l
            WHERE m."Id" = l.old_id;
            """;

        private const string ResetMerchantSequences = """
            SELECT setval(pg_get_serial_sequence('"Merchants"', 'Id'),
                          GREATEST((SELECT COALESCE(MAX("Id"), 1) FROM "Merchants"), 1));
            SELECT setval(pg_get_serial_sequence('"MerchantAliases"', 'Id'),
                          GREATEST((SELECT COALESCE(MAX("Id"), 1) FROM "MerchantAliases"), 1));
            """;

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "MerchantAliases",
                keyColumn: "Id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1013);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1014);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1028);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1029);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1030);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1031);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1032);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1033);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1034);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1035);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1036);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1037);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1038);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1039);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1040);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1041);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1042);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1043);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1044);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1045);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1046);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1047);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1048);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1049);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1050);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1051);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1052);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1053);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1054);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1055);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1056);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1057);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1058);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1059);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1060);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1061);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1062);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1063);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1064);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1065);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1066);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1067);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1068);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1069);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1070);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1071);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1072);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1073);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1074);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1075);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1076);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1077);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1078);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1079);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1080);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1081);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1082);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1083);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1084);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1085);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1086);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1087);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1088);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1089);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1090);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1091);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1092);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1093);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1094);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1095);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1096);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1097);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1098);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1099);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1100);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1101);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1102);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1103);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1104);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1105);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1106);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1107);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1108);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1109);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1110);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1111);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1112);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1113);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1114);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1115);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1116);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1117);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1118);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1119);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1120);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1121);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1122);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1123);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1124);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1125);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1126);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1127);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1128);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1129);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1130);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1131);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1132);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1133);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1134);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1135);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1136);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1137);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1138);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1139);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1140);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1141);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1142);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1143);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1144);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1145);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1146);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1147);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1148);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1149);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1150);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1151);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1152);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1153);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1154);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1155);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1156);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1157);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1158);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1159);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1160);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1161);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1162);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1163);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1164);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1165);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1166);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1167);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1168);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1169);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1170);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1171);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1172);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1173);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1174);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1175);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1176);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1177);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1178);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1179);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1180);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1181);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1182);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1183);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1184);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1185);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1186);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1187);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1188);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1189);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1190);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1191);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1192);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1193);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1194);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1195);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1196);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1197);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1198);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1199);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1200);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1201);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1202);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1203);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1204);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1205);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1206);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1207);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1208);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1209);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1210);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1211);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1212);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1213);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1214);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1215);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1216);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1217);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1218);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1219);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1220);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1221);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1222);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1223);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1224);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1225);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1226);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1227);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1228);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1229);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1230);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1231);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1232);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1233);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1234);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1235);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1236);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1237);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1238);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1239);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1240);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1241);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1242);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1243);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1244);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1245);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1246);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1247);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1248);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1249);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1250);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1251);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1252);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1253);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1254);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1255);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1256);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1257);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1258);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1259);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1260);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1261);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1262);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1263);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1264);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1265);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1266);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1267);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1268);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1269);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1270);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1271);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1272);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1273);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1274);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1275);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1276);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1277);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1278);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1279);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1280);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1281);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1282);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1283);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1284);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1285);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1286);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1287);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1288);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1289);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1290);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1291);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1292);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1293);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1294);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1295);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1296);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1297);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1298);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1299);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1300);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1301);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1302);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1303);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1304);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1305);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1306);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1307);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1308);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1309);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1310);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1311);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1312);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1313);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1314);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1315);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1316);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1317);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1318);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1319);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1320);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1321);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1322);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1323);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1324);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1325);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1326);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1327);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1328);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1329);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1330);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1331);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1332);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1333);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1334);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1335);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1336);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1337);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1338);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1339);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1340);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1341);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1342);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1343);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1344);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1345);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1346);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1347);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1348);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1349);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1350);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1351);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1352);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1353);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1354);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1355);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1356);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1357);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1358);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1359);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1360);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1361);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1362);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1363);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1364);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1365);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1366);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1367);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1368);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1369);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1370);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1371);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1372);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1373);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1374);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1375);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1376);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1377);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1378);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1379);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1380);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1381);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1382);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1383);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1384);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1385);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1386);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1387);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1388);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1389);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1390);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1391);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1392);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1393);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1394);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1395);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1396);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1397);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1398);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1399);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1400);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1401);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1402);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1403);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1404);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1405);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1406);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1407);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1408);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1409);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1410);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1411);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1412);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1413);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1414);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1415);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1416);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1417);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1418);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1419);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1420);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1421);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1422);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1423);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1424);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1425);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1426);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1427);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1428);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1429);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1430);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1431);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1432);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1433);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1434);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1435);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1436);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1437);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1438);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1439);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1440);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1441);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1442);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1443);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1444);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1445);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1446);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1447);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1448);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1449);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1450);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1451);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1452);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1453);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1454);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1455);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1456);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1457);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1458);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1459);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1460);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1461);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1462);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1463);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1464);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1465);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1466);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1467);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1468);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1469);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1470);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1471);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1472);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1473);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1474);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1475);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1476);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1477);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1478);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1479);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1480);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1481);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1482);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1483);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1484);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1485);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1486);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1487);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1488);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1489);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1490);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1491);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1492);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1493);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1494);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1495);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1496);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1497);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1498);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1499);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1500);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1501);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1502);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1503);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1504);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1505);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1506);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1507);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1508);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1509);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1510);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1511);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1512);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1513);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1514);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1515);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1516);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1517);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1518);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1519);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1520);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1521);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1522);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1523);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1524);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1525);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1526);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1527);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1528);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1529);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1530);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1531);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1532);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1533);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1534);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1535);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1536);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1537);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1538);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1539);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1540);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1541);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1542);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1543);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1544);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1545);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1546);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1547);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1548);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1549);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1550);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1551);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1552);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1553);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1554);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1555);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1556);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1557);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1558);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1559);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1560);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1561);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1562);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1563);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1564);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1565);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1566);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1567);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1568);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1569);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1570);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1571);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1572);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1573);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1574);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1575);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1576);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1577);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1578);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1579);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1580);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1581);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1582);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1583);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1584);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1585);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1586);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1587);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1588);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1589);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1590);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1591);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1592);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1593);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1594);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1595);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1596);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1597);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1598);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1599);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1600);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1601);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1602);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1603);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1604);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1605);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1606);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1607);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1608);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1609);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1610);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1611);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1612);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1613);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1614);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1615);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1616);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1617);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1618);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1619);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1620);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1621);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1622);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1623);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1624);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1625);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1626);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1627);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1628);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1629);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1630);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1631);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1632);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1633);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1634);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1635);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1636);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1637);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1638);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1639);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1640);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1641);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1642);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1643);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1644);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1645);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1646);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1647);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1648);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1649);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1650);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1651);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1652);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1653);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1654);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1655);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1656);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1657);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1658);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1659);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1660);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1661);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1662);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1663);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1664);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1665);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1666);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1667);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1668);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1669);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1670);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1671);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1672);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1673);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1674);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1675);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1676);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1677);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1678);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1679);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1680);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1681);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1682);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1683);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1684);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1685);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1686);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1687);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1688);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1689);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1690);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1691);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1692);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1693);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1694);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1695);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1696);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1697);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1698);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1699);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1700);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1701);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1702);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1703);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1704);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1705);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1706);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1707);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1708);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1709);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1710);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1711);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1712);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1713);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1714);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1715);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1716);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1717);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1718);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1719);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1720);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1721);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1722);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1723);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1724);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1725);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1726);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1727);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1728);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1729);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1730);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1731);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1732);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1733);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1734);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1735);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1736);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1737);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1738);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1739);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1740);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1741);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1742);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1743);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1744);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1745);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1746);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1747);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1748);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1749);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1750);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1751);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1752);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1753);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1754);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1755);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1756);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1757);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1758);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1759);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1760);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1761);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1762);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1763);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1764);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1765);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1766);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1767);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1768);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1769);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1770);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1771);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1772);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1773);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1774);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1775);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1776);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1777);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1778);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1779);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1780);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1781);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1782);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1783);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1784);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1785);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1786);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1787);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1788);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1789);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1790);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1791);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1792);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1793);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1794);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1795);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1796);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1797);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1798);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1799);

            migrationBuilder.DeleteData(
                table: "Merchants",
                keyColumn: "Id",
                keyValue: 1800);
        }
    }
}
