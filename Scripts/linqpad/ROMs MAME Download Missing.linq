<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "includes\HttpUtils"
#load "includes\RomUtils"

var romPath = @"X:\New\mame";
var missingROMs = new List<string>
{
	"ad_59mc07",
	"agvision",
	"amhbattl",
	"ap10",
	"ax200",
	"ax208",
	"ax208p",
	"ax230",
	"ax350ii",
	"ax350iif",
	"ax500",
	"bbbingot",
	"bgareggablj",
	"bgareggak",
	"bigkonggx",
	"bingopd",
	"bloxeeda",
	"bmxstunts",
	"brapboysp2",
	"bruc100a",
	"c2color",
	"canonv20",
	"canonv8",
	"cf2700uk",
	"champbwla",
	"clowns_audio",
	"cmfb55",
	"conquest",
	"cpc61",
	"ctk601",
	"cx5f",
	"cx5m128",
	"cx5mu",
	"dance864",
	"ddr2mlb",
	"ddrkara2",
	"ddrsn2a",
	"ddrsn2aa",
	"ddrsn2j",
	"ddrsn2ja",
	"ddrsn2u",
	"ddrsna",
	"ddrsnj",
	"ddrsnu",
	"deroona",
	"dmxa",
	"docchift",
	"dominoa",
	"dpc200e",
	"drmn10ma",
	"drmn9ma",
	"drmn9mb",
	"drmnv2a",
	"drmnv2aa",
	"drmnv2ab",
	"drmnv2j",
	"drmnv2ja",
	"drmnv2jb",
	"drmnv3a",
	"drmnv3aa",
	"drmnv3j",
	"drmnv3ja",
	"drmnva",
	"drmnvj",
	"dstagesn",
	"dstagesna",
	"dx7",
	"dx9",
	"dygolfp",
	"elcirculo",
	"electron_voxbox",
	"eshockey",
	"f4431",
	"f4431_kbd",
	"famitvc1",
	"fantasy_sound",
	"fantcar",
	"fantcar87",
	"fctitler",
	"fruit_samples",
	"fsa1",
	"fsa1fx",
	"fspc800",
	"galactica2",
	"gmissile_audio",
	"goldenchip",
	"gradius4a",
	"gradius4aa",
	"gradius4ja",
	"gradius4u",
	"gradius4ua",
	"gtfrk11ma",
	"gtgt21",
	"gtrfrk5ma",
	"gtrfrksaa",
	"gtrfrksea",
	"gtrfrksja",
	"gtrfrksua",
	"gtrfrkv2a",
	"gtrfrkv2aa",
	"gtrfrkv2ab",
	"gtrfrkv2j",
	"gtrfrkv2ja",
	"gtrfrkv2jb",
	"gtrfrkv3a",
	"gtrfrkv3aa",
	"gtrfrkv3j",
	"gtrfrkv3ja",
	"gtrfrkva",
	"gtrfrkvj",
	"gtsers18",
	"gz70sp",
	"hb101",
	"hb20p",
	"hb75",
	"hbf9pr",
	"hitpar",
	"hkittymp",
	"holddraw",
	"hx21f",
	"hx23f",
	"hx32",
	"hx51i",
	"ibarao",
	"j_lal",
	"j_supsha",
	"kbottom9",
	"kokoroja",
	"kov3hd101",
	"kov3hd102",
	"kov3hd103",
	"kov3hd104",
	"kpython2",
	"lanteach",
	"lantrans",
	"lantransp",
	"letterlf",
	"letterlg",
	"luna88k2",
	"m4_audio",
	"m_bappl2",
	"m_bapple",
	"m_gndgit",
	"m_lndg",
	"m_mtchit",
	"m_mtchup",
	"magicpop",
	"mastboyiv2",
	"mav_200",
	"mbh1",
	"mbh3",
	"mg1_kbd_device",
	"minigolfa",
	"ml8000",
	"mlg30_2",
	"mltpoints",
	"mmahjong",
	"mmhammer",
	"mononcol",
	"monsterb_sound",
	"mpc10",
	"mpc25fs",
	"mps1200",
	"mps1250",
	"mspackpls",
	"mx15",
	"n501",
	"nes_bandai_pt554",
	"nes_jf13",
	"nes_jf17_pcm",
	"nes_jf19_pcm",
	"nes_jf23",
	"nes_jf24",
	"nes_jf29",
	"nes_jf33",
	"nesm8",
	"nesm8a",
	"nesm8b",
	"netchu02c",
	"newxpanga",
	"nf_10f",
	"nms8245",
	"nms8250_16",
	"nms8250_19",
	"nws5000x",
	"pcnfrk10ma",
	"pcnfrk9ma",
	"perlacrb",
	"phantom2_audio",
	"phc2",
	"phc23jb",
	"phusion",
	"pitapata",
	"psr500",
	"pss12",
	"punchkid",
	"quizardi",
	"quizardi_10",
	"quizardi_12",
	"quizardi_17",
	"qx10_keyboard_hasci",
	"redearthn",
	"redearthnr1",
	"rmontecarlo",
	"rs6k7009",
	"rs_pa2",
	"s1945iiibl",
	"s97271p",
	"saladue",
	"saladuea",
	"sasuke_sound",
	"satansat_sound",
	"scruiser",
	"seawolf_audio",
	"simpsons4pe2",
	"sinvemag2",
	"slepachi",
	"smb3bl",
	"soulclbraa",
	"spellabc",
	"srmagic",
	"sscope2a",
	"sscope2ab",
	"sscope2abvd1",
	"sscope2ac",
	"sscope2acvd1",
	"sscope2avd1",
	"sscope2e",
	"sscope2eb",
	"sscope2ebvd1",
	"sscope2ec",
	"sscope2ecvd1",
	"sscope2evd1",
	"sscope2j",
	"sscope2jb",
	"sscope2jbvd1",
	"sscope2jc",
	"sscope2jcvd1",
	"sscope2jvd1",
	"sscope2ubvd1",
	"sscope2ucvd1",
	"svi728es",
	"sx100",
	"tadpc200",
	"tadpc200b",
	"tdracula",
	"teammate",
	"tgaiden3",
	"tgargnf",
	"thrild3j",
	"tmegaman3",
	"tokyocopi",
	"toysmarch",
	"toysmarch2",
	"trojanlt",
	"trsvidtx",
	"trvmstrd",
	"tsimquest",
	"tsuperman",
	"turbofrcua",
	"tvboy",
	"twinsa",
	"ucv102",
	"ufo7",
	"unk960606b",
	"unkhockey",
	"usg211c",
	"vanguard_sound",
	"vg8020f",
	"victhc80",
	"vl1",
	"westdrmg",
	"y503iir",
	"yis503",
	"yis503ii",
	"zoomania",
	"zoomaniaa",
}
.Select(r => $"{r}.zip");

await DownloadMissingMAME(missingROMs, romPath);
