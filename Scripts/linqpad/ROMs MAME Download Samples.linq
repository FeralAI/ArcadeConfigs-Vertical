<Query Kind="Statements">
  <Namespace>System.Net.Http</Namespace>
</Query>

#load "includes\HttpUtils"
#load "includes\RomUtils"

var romPath = @"X:\ROMs\mame-samples";
var missingROMs = new List<string>
{
	"005.zip",
	"3bagflvt.zip",
	"MM1_keyboard.zip",
	"astrof.zip",
	"battles.zip",
	"bbc.zip",
	"blockade.zip",
	"bowl3d.zip",
	"buckrog.zip",
	"carnival.zip",
	"circus.zip",
	"clowns.zip",
	"congo.zip",
	"cosmica.zip",
	"cosmicg.zip",
	"crash.zip",
	"dai3wksi.zip",
	"depthch.zip",
	"equites.zip",
	"fantasy.zip",
	"fruitsamples.zip",
	"ftaerobi.zip",
	"gaplus.zip",
	"genpin.zip",
	"gmissile.zip",
	"gridlee.zip",
	"homerun.zip",
	"invaders.zip",
	"invinco.zip",
	"ipminvad.zip",
	"journey.zip",
	"lrescue.zip",
	"lupin3.zip",
	"m4.zip",
	"mmagic.zip",
	"moepro.zip",
	"moepro88.zip",
	"moepro90.zip",
	"monsterb.zip",
	"mpsaikyo.zip",
	"mptennis.zip",
	"natodef.zip",
	"nsub.zip",
	"ozmawars.zip",
	"panic.zip",
	"phantom2.zip",
	"ptrmj.zip",
	"pulsar.zip",
	"qbert.zip",
	"rallyx.zip",
	"redclash.zip",
	"relay.zip",
	"ripcord.zip",
	"robotbwl.zip",
	"safarir.zip",
	"sasuke.zip",
	"seawolf.zip",
	"sharkatt.zip",
	"smoepro.zip",
	"spacefb.zip",
	"spaceod.zip",
	"subroc3d.zip",
	"targ.zip",
	"tattack.zip",
	"terao.zip",
	"thehand.zip",
	"thief.zip",
	"triplhnt.zip",
	"turbo.zip",
	"twotiger.zip",
	"vanguard.zip",
	"zaxxon.zip",
	"zerohour.zip",
};

await DownloadMissingMAMESamples(missingROMs, romPath);
