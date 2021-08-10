using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Network
{
    public enum Opcode
    {
		// TODO: add all out!

		// Global
		KEY_EXCHANGE = 1,
		KEY_EXCHANGE_RSP = 2,

		// Auth
		AUTH_LOGIN = 500,						// in
		AUTH_LOGIN_RSP = 501,					// out
		AUTH_LOGIN_FAIL = 502,					// out
		// 503									// in

		// Square					
		LOBBY_LOGIN = 600,						// in
		LOBBY_LOGIN_RSP = 601,					// out
		LOBBY_LOGIN_FAIL = 602,					// out
		LOBBY_BUDDY_SET_ONLINE = 603,			// out
		// 604									// in
		LOBBY_BUDDY_SET_OFFLINE = 607,			// out

		LOBBY_EQUIP_ARMOUR = 620,				// in
		LOBBY_EQUIP_ARMOUR_RSP = 621,			// out

		LOBBY_EQUIP_WEAPON = 623,				// in
		LOBBY_EQUIP_WEAPON_RSP = 624,           // out
		// 631									// in
		
		LOBBY_EQUIPPED_SKILLS = 646,			// in
		LOBBY_EQUIPPED_SKILS_RSP = 647,			// out
		// 649									// in
		LOBBY_DROP_CARD = 652,					// in
		LOBBY_DROP_CARD_RSP = 653,				// out 
		LOBBY_OPEN_GM_CARD = 655,				// in
		LOBBY_OPEN_GM_CARD_RSP = 656,			// out
		
		LOBBY_UPDATE_ACCOUNT = 662,

		LOBBY_REGISTER_NAME = 667,				// in
		LOBBY_REGISTER_NAME_RSP = 668,			// out

		LOBBY_SERVER_ERROR = 669,

		LOBBY_REGISTER_CHARACTERS = 670,		// in
		LOBBY_REGISTER_CHARACTERS_RSP = 671,
		// 676									// in
		LOBBY_SWAP_CHARACTER = 679,				// in
		LOBBY_SWAP_CHARACTER_RSP = 680,			// out

		LOBBY_PLAYERINFO = 691,					// in
		LOBBY_PLAYERINFO_RSP = 692,				// out

		LOBBY_BUDDIES = 694,					// in
		LOBBY_BUDDIES_RSP = 695,				// out	
		LOBBY_FRIEND_INVITE = 697,				// in
		LOBBY_FRIEND_INVITE_RSP = 698,			// out
		LOBBY_FRIEND_INVITE_FAIL = 699,			// out	

		LOBBY_ADD_INCOMING_FRIEND = 700,
		LOBBY_ACCEPT_INCOMING_FRIEND = 701,		// in
		LOBBY_ACCEPT_INCOMING_FRIEND_RSP = 702, // out
		LOBBY_OUTGOING_FRIEND_ACCEPTED = 704,
		LOBBY_DENY_INCOMING_FRIEND = 705,		// in
		LOBBY_DENY_INCOMING_FRIEND_RSP = 706,	// out
		LOBBY_OUTGOING_FRIEND_DENIED = 708,

		LOBBY_REMOVE_OUTGOING_FRIEND = 709,
		LOBBY_REMOVE_OUTGOING_FRIEND_RSP = 710,

		LOBBY_REMOVE_INCOMING_FRIEND = 712,
		// 713									// in

		LOBBY_REMOVE_FRIEND = 713,
		LOBBY_REMOVE_FRIEND_RSP = 714,
		LOBBY_FRIEND_REMOVE_FRIEND = 716,
		// 717									// in

		LOBBY_RECEIVE_INVITE = 717,
		LOBBY_RANDOM_INVITE = 721,				// in


		LOBBY_MEMOS = 725,						// in
		LOBBY_MEMOS_RSP = 726,					// out
		// 727									// out
		LOBBY_SEND_MEMO = 728,					// in
		LOBBY_SEND_MEMO_RSP = 729,				// out
		LOBBY_SEND_MEMO_FAIL = 730,				// out
		LOBBY_RECEIVE_MEMO = 731,				// out
		LOBBY_OPEN_MEMO = 732,					// int
		LOBBY_OPEN_MEMO_RSP = 733,				// out

		LOBBY_DELETE_MEMO = 735,				// in
		LOBBY_DELETE_MEMO_RSP = 736,			// out

		LOBBY_WHISPER = 738,					// in
		LOBBY_WHISPER_RSP = 739,				// out
		LOBBY_WHISPER_FAIL = 740,				// out

		LOBBY_OPEN_PLAYER_CARD = 742,			// in
		LOBBY_OPEN_PLAYER_CARD_RSP = 743,		// out

		//Option cards [not settings]
		LOBBY_GIFTS = 745,						// in
		LOBBY_GIFTS_RSP = 746,					// out

		LOBBY_ROOM_REQUEST_STATS = 751,			// in
		LOBBY_ROOM_REQUEST_STATS_RSP = 752,		// out

		// 754									// in

		LOBBY_GAMEROOMS = 758,					// in
		LOBBY_GAMEROOMS_RSP = 759,				// out

		LOBBY_CHANNELS = 762,					// in
		LOBBY_CHANNELS_RSP = 763,				// out

		LOBBY_CHANNEL_CONNECT = 766,			// in
		LOBBY_CHANNEL_CONNECT_RSP = 767,		// out
		LOBBY_GAMESERVER_REFRESH = 769,			// in
		LOBBY_GAMESERVER_REFRESH_RSP = 770,		// out

		LOBBY_REGISTER_CARD_FAIL = 779,
		// 797									// in
		// 780									// in
		LOBBY_INVENTORY_CARDS = 780,			// in
		LOBBY_INVENTORY_CARDS_RSP = 781,		// out
		// 785									// in

		LOBBY_PLAYER_RANKING = 791,				// in
		LOBBY_PLAYER_RANKING_RSP = 792,			// out

		LOBBY_REQUEST_GOODS = 797,				// in
		LOBBY_NORMAL_GOODS = 798,				// out

		LOBBY_REQUEST_PACKAGE_GOODS = 800,		// in
		LOBBY_REQUEST_PACKAGE_GOODS_RSP = 801,	// out
		LOBBY_BUY_ITEM = 803,					// in
		LOBBY_BUY_ITEM_RSP = 804,				// out
		LOBBY_BUY_ITEM_FAIL = 805,				// out
		// 806									// in
		LOBBY_GIFT_ITEM = 812,					// in
		LOBBY_GIFT_ITEM_RSP = 813,				// out
		LOBBY_GIFT_ITEM_FAIL = 814,				// out
		LOBBY_SHOP_GIFT = 815,					// in
		LOBBY_SHOP_GIFT_RSP = 816,				// out
		// 818									// in

		LOBBY_SHOP_GIFT_FAIL = 820,
		LOBBY_RECEIVE_GIFT = 821,
		LOBBY_TUTORIAL_PART_FINISH = 822,		// in
		LOBBY_TUTORIAL_PART_FINISH_RSP = 823,	// out
		// 825									// in
		// 828									// in

		LOBBY_REQUEST_CASH = 831,				// in
		LOBBY_REQUEST_CASH_RSP = 832,			// out

		LOBBY_OPEN_FUNC_CARD_FAIL = 833,
		// 834									// in
		LOBBY_REGISTRATION_FAIL = 836,
		// 837									// in

		LOBBY_RECORD_CLEAR_FAIL = 840,
		// 841									// in
		LOBBY_KILL_DEATH_CLEAR_FAIL = 843,
		// 844									// in

		LOBBY_REDEEM_CODE = 851,				// in
		// 852									// out
		// 853									// out (msg?)

		// 854 // bidyy pending

		LOBBY_RECEIVE_GM_INVITE = 857,			// in
		// 860 GM invite?
		// 861									// in
		LOBBY_USE_FUNC_CARD_RSP = 864,

		LOBBY_BUDDY_ADD_FAIL = 865,
		// 866									// in
		// 867 unk func card
		// 868 buddy no more 
		// 869									// in
		// 872									// in
		LOBBY_TRADE_ERROR_1 = 874,
		// 875									// in

		LOBBY_TRADE = 875,						// in
		LOBBY_TRADE_RSP = 876,					// out

		// 877 trading now / not want trade
		LOBBY_TRADE_ERROR_2 = 878,	
		// 879									// in
		// 881 TRADE_REQUEST_CANCEL
		// 882 TRADE_REQUEST_REJECT_MESSAGE
		// 884									// in
		// 885 trade cancle


		// 887 unk trade error
		// 888 unk								// in
		// 889 unk
		LOBBY_TRADE_ACT = 884, // cancle trade?
		LOBBY_TRADE_ACT_RSP = 885,

		LOBBY_TRADE_ERROR_3 = 887,
		LOBBY_TRADE_ERROR_4 = 890,

		LOBBY_USE_CRANE = 897,					// in
		LOBBY_USE_CRANE_RSP = 898,
		LOBBY_USE_CRAIN_FAIL = 899,

		// 901									// in
		LOBBY_ENCHANT_FAIL = 902,
		// 903									// in

		LOBBY_CHEST_OPEN = 906, // TODO?		// in

		LEET_ANTI_CHEET = 1337, // CUSTON		// in

		SQUARE_LOGIN = 6500,					// in
		SQUARE_LOGIN_RSP = 6501,				// out

		SQUARE_PLAYER_JOIN = 6506,				// in
		SQUARE_ADD_PLAYER = 6507,				// out
		SQUARE_LOAD_PLAYERS = 6508,
		SQUARE_REMOVE_PLAYER = 6509,
		SQUARE_PLAYER_MOVE = 6510,				// in
		SQUARE_MOVE_PLAYER = 6513,
		SQUARE_PLAYER_UPDATE = 6514,			// in
		SQUARE_UPDATE_PLAYER = 6517,
		SQUARE_CHAT = 6526,						// in
		SQUARE_CHAT_RSP = 6529,					// out
		SQUARE_JOIN_BACK = 6530,				// in
		SQUARE_JOIN_BACK_RSP = 6531,			// out
		SQUARE_JOIN_PARK = 6537,				// in
		SQUARE_JOIN_PARK_RSP = 6538,			// out
		SQUARE_JOIN_PARK_FAIL = 6539,			// out
		SQUARE_UPDATE_LIST = 6543,				// out
		SQUARE_START_TUTORIAL = 6544,			// in
		SQUARE_PLAYER_CHANGE_STATE = 6547,		// out
		// 6549									// in
		// 6552									// out
		SQUARE_PLAYER_CHANGE_LEVEL = 6553,		// in
		SQUARE_PLAYER_CHANGE_CHANNEL = 6554,	// in
		SQUARE_PLAYER_CHANGE_CHANNEL_RSP = 6555,// out
		// 6556									// out
		SQUARE_PLAYER_EMOTE = 6557,				// in
		SQUARE_EMOTE_PLAYER = 6558,				// out

		// 28?									// in
	}
}
