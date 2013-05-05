﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using HabboEvents;
using Firewind.Core;

namespace Firewind.Messages.StaticMessageHandlers
{
    class StaticClientMessageHandler
    {
        private delegate void StaticRequestHandler(GameClientMessageHandler handler);
        private static Hashtable handlers;
        private static List<int> unknownPackets = new List<int>();

        internal static void Initialize()
        {
            handlers = new Hashtable();
            RegisterPacketLibary();
        }

        internal static void HandlePacket(GameClientMessageHandler handler, ClientMessage message)
        {

            if (handlers.ContainsKey(message.Id))
            {
                if (FirewindEnvironment.IsDebugging)
                    Logging.LogDebug("Event handled => " + message.Id + " ");
                StaticRequestHandler currentHandler = (StaticRequestHandler)handlers[message.Id];
                currentHandler.Invoke(handler);
            }
            else
            {
                if (!FirewindEnvironment.IsDebugging/* || unknownPackets.Contains(message.Id)*/)
                    return;
                unknownPackets.Add(message.Id);
                Logging.LogDebug("Unknown packet ID: " + message.Id);
            }
        }

        #region Register
        internal static void RegisterPacketLibary()
        {
            handlers.Add(Incoming.CheckReleaseMessageEvent, new StaticRequestHandler(SharedPacketLib.CheckHabboRelease));
            handlers.Add(Incoming.InitCrypto, new StaticRequestHandler(SharedPacketLib.InitCrypto));
            handlers.Add(Incoming.SecretKey, new StaticRequestHandler(SharedPacketLib.SecretKey));
            handlers.Add(Incoming.ClientVars, new StaticRequestHandler(SharedPacketLib.setVars));
            handlers.Add(Incoming.UniqueMachineID, new StaticRequestHandler(SharedPacketLib.setUniqueId));
            handlers.Add(Incoming.PrepareCampaing, new StaticRequestHandler(SharedPacketLib.PrepareCampaing));
            handlers.Add(Incoming.SendCampaingData, new StaticRequestHandler(SharedPacketLib.SendCampaingData));
            handlers.Add(Incoming.OpenCatalog, new StaticRequestHandler(SharedPacketLib.GetCatalogIndex));
            handlers.Add(Incoming.OpenCatalogPage, new StaticRequestHandler(SharedPacketLib.GetCatalogPage));
            handlers.Add(Incoming.RedeemVoucher, new StaticRequestHandler(SharedPacketLib.RedeemVoucher));
            handlers.Add(Incoming.PurchaseCatalogItem, new StaticRequestHandler(SharedPacketLib.HandlePurchase));
            handlers.Add(Incoming.PurchaseFromCatalogAsGift, new StaticRequestHandler(SharedPacketLib.PurchaseFromCatalogAsGift));
            handlers.Add(Incoming.GetRecyclerPrizes, new StaticRequestHandler(SharedPacketLib.GetRecyclerRewards));
            handlers.Add(Incoming.CatalogData1, new StaticRequestHandler(SharedPacketLib.GetCataData1));
            handlers.Add(Incoming.CatalogData2, new StaticRequestHandler(SharedPacketLib.GetCataData2));
            handlers.Add(Incoming.CheckPetName, new StaticRequestHandler(SharedPacketLib.CheckPetName));
            handlers.Add(Incoming.CatalogGetRace, new StaticRequestHandler(SharedPacketLib.PetRaces));
            handlers.Add(Incoming.Pong, new StaticRequestHandler(SharedPacketLib.Pong));
            handlers.Add(Incoming.SSOTicket, new StaticRequestHandler(SharedPacketLib.SSOLogin));
            handlers.Add(Incoming.OpenHelpTool, new StaticRequestHandler(SharedPacketLib.InitHelpTool));
            handlers.Add(Incoming.CreateTicket, new StaticRequestHandler(SharedPacketLib.SubmitHelpTicket));
            handlers.Add(Incoming.SendRoomAlert, new StaticRequestHandler(SharedPacketLib.ModSendRoomAlert));
            handlers.Add(Incoming.PickIssue, new StaticRequestHandler(SharedPacketLib.ModPickTicket));
            handlers.Add(Incoming.ReleaseIssue, new StaticRequestHandler(SharedPacketLib.ModReleaseTicket));
            handlers.Add(Incoming.CloseIssues, new StaticRequestHandler(SharedPacketLib.CloseIssues));
            handlers.Add(Incoming.ToolForUser, new StaticRequestHandler(SharedPacketLib.ModGetUserInfo));
            handlers.Add(Incoming.UserChatlog, new StaticRequestHandler(SharedPacketLib.ModGetUserChatlog));
            handlers.Add(Incoming.OpenRoomChatlog, new StaticRequestHandler(SharedPacketLib.ModGetRoomChatlog));
            handlers.Add(Incoming.IssueChatlog, new StaticRequestHandler(SharedPacketLib.ModGetTicketChatlog));
            handlers.Add(Incoming.GetRoomVisits, new StaticRequestHandler(SharedPacketLib.ModGetRoomVisits));
            handlers.Add(Incoming.ToolForThisRoom, new StaticRequestHandler(SharedPacketLib.ModGetRoomTool));
            handlers.Add(Incoming.PerformRoomAction, new StaticRequestHandler(SharedPacketLib.ModPerformRoomAction));
            handlers.Add(Incoming.SendUserMessage, new StaticRequestHandler(SharedPacketLib.ModSendUserCaution));
            handlers.Add(Incoming.SendMessageByTemplate, new StaticRequestHandler(SharedPacketLib.ModSendUserMessage));
            handlers.Add(Incoming.ModActionKickUser, new StaticRequestHandler(SharedPacketLib.ModKickUser));
            handlers.Add(Incoming.ModActionMuteUser, new StaticRequestHandler(SharedPacketLib.ModKickUser));
            handlers.Add(Incoming.ModActionBanUser, new StaticRequestHandler(SharedPacketLib.ModBanUser));
            handlers.Add(Incoming.UpdateFriendsState, new StaticRequestHandler(SharedPacketLib.FriendsListUpdate));
            handlers.Add(Incoming.DeleteFriend, new StaticRequestHandler(SharedPacketLib.RemoveBuddy));
            handlers.Add(Incoming.SearchFriend, new StaticRequestHandler(SharedPacketLib.SearchHabbo));
            handlers.Add(Incoming.SendInstantMessenger, new StaticRequestHandler(SharedPacketLib.SendInstantMessenger));
            handlers.Add(Incoming.AcceptRequest, new StaticRequestHandler(SharedPacketLib.AcceptRequest));
            handlers.Add(Incoming.DeclineFriend, new StaticRequestHandler(SharedPacketLib.DeclineFriend));
            handlers.Add(Incoming.FriendRequest, new StaticRequestHandler(SharedPacketLib.RequestBuddy));
            handlers.Add(Incoming.FollowFriend, new StaticRequestHandler(SharedPacketLib.FollowBuddy));
            handlers.Add(Incoming.InviteFriendsToMyRoom, new StaticRequestHandler(SharedPacketLib.SendInstantInvite));
            handlers.Add(Incoming.AddFavourite, new StaticRequestHandler(SharedPacketLib.AddFavorite));
            handlers.Add(Incoming.RemoveFavourite, new StaticRequestHandler(SharedPacketLib.RemoveFavorite));
            handlers.Add(Incoming.GoToHotelView, new StaticRequestHandler(SharedPacketLib.GoToHotelView));
            handlers.Add(Incoming.LoadCategorys, new StaticRequestHandler(SharedPacketLib.GetFlatCats));
            handlers.Add(Incoming.LoadFeaturedRooms, new StaticRequestHandler(SharedPacketLib.GetPubs));
            handlers.Add(Incoming.PopularRoomsSearch, new StaticRequestHandler(SharedPacketLib.PopularRoomsSearch));
            handlers.Add(Incoming.HighRatedRooms, new StaticRequestHandler(SharedPacketLib.GetHighRatedRooms));
            handlers.Add(Incoming.RoomsOfMyFriends, new StaticRequestHandler(SharedPacketLib.GetFriendsRooms));
            handlers.Add(Incoming.RoomsWhereMyFriends, new StaticRequestHandler(SharedPacketLib.GetRoomsWithFriends));
            handlers.Add(Incoming.LoadMyRooms, new StaticRequestHandler(SharedPacketLib.GetOwnRooms));
            handlers.Add(Incoming.MyFavs, new StaticRequestHandler(SharedPacketLib.GetFavoriteRooms));
            handlers.Add(Incoming.RecentRooms, new StaticRequestHandler(SharedPacketLib.GetRecentRooms));
            handlers.Add(Incoming.LoadPopularTags, new StaticRequestHandler(SharedPacketLib.GetPopularTags));
            handlers.Add(Incoming.SearchRoomByName, new StaticRequestHandler(SharedPacketLib.PerformSearch));
            handlers.Add(Incoming.Talk, new StaticRequestHandler(SharedPacketLib.Talk));
            handlers.Add(Incoming.Shout, new StaticRequestHandler(SharedPacketLib.Shout));
            handlers.Add(Incoming.Whisp, new StaticRequestHandler(SharedPacketLib.Whisper));
            handlers.Add(Incoming.Move, new StaticRequestHandler(SharedPacketLib.Move));
            handlers.Add(Incoming.CanCreateRoom, new StaticRequestHandler(SharedPacketLib.CanCreateRoom));
            handlers.Add(Incoming.CreateRoom, new StaticRequestHandler(SharedPacketLib.CreateRoom));
            handlers.Add(Incoming.OpenFlatConnection, new StaticRequestHandler(SharedPacketLib.OpenFlatConnection));
            handlers.Add(Incoming.GetRoomSettings, new StaticRequestHandler(SharedPacketLib.GetRoomSettings));
            handlers.Add(Incoming.SaveRoomSettings, new StaticRequestHandler(SharedPacketLib.SaveRoomSettings));
            handlers.Add(Incoming.GiveRights, new StaticRequestHandler(SharedPacketLib.GiveRights));
            handlers.Add(Incoming.RemoveRights, new StaticRequestHandler(SharedPacketLib.RemoveRights));
            handlers.Add(Incoming.RemoveAllRights, new StaticRequestHandler(SharedPacketLib.TakeAllRights));
            handlers.Add(Incoming.KickUserOfRoom, new StaticRequestHandler(SharedPacketLib.KickUser));
            handlers.Add(Incoming.BanUserOfRoom, new StaticRequestHandler(SharedPacketLib.BanUser));
            handlers.Add(Incoming.StartTrade, new StaticRequestHandler(SharedPacketLib.InitTrade));
            handlers.Add(Incoming.SetHome, new StaticRequestHandler(SharedPacketLib.SetHomeRoom));
            handlers.Add(Incoming.RemoveRoom, new StaticRequestHandler(SharedPacketLib.DeleteRoom));
            handlers.Add(Incoming.LookTo, new StaticRequestHandler(SharedPacketLib.LookAt));
            handlers.Add(Incoming.StartTyping, new StaticRequestHandler(SharedPacketLib.StartTyping));
            handlers.Add(Incoming.StopTyping, new StaticRequestHandler(SharedPacketLib.StopTyping));
            handlers.Add(Incoming.IgnoreUser, new StaticRequestHandler(SharedPacketLib.IgnoreUser));
            handlers.Add(Incoming.UnignoreUser, new StaticRequestHandler(SharedPacketLib.UnignoreUser));
            handlers.Add(Incoming.CanCreateRoomEvent, new StaticRequestHandler(SharedPacketLib.CanCreateRoomEvent));
            handlers.Add(Incoming.CreateRoomEvent, new StaticRequestHandler(SharedPacketLib.StartEvent));
            handlers.Add(Incoming.StopEvent, new StaticRequestHandler(SharedPacketLib.StopEvent));
            handlers.Add(Incoming.EditEvent, new StaticRequestHandler(SharedPacketLib.EditEvent));
            handlers.Add(Incoming.ApplyAction, new StaticRequestHandler(SharedPacketLib.Wave));
            handlers.Add(Incoming.ApplySign, new StaticRequestHandler(SharedPacketLib.Sign));
            handlers.Add(Incoming.GetUserTags, new StaticRequestHandler(SharedPacketLib.GetUserTags));
            handlers.Add(Incoming.GetUserBadges, new StaticRequestHandler(SharedPacketLib.GetUserBadges));
            handlers.Add(Incoming.GiveRoomScore, new StaticRequestHandler(SharedPacketLib.RateRoom));
            handlers.Add(Incoming.ApplyDance, new StaticRequestHandler(SharedPacketLib.Dance));
            handlers.Add(Incoming.RemoveHanditem, new StaticRequestHandler(SharedPacketLib.RemoveHanditem));
            handlers.Add(Incoming.GiveObject, new StaticRequestHandler(SharedPacketLib.GiveHanditem));
            handlers.Add(Incoming.AnswerDoorBell, new StaticRequestHandler(SharedPacketLib.AnswerDoorbell));
            handlers.Add(Incoming.ApplySpace, new StaticRequestHandler(SharedPacketLib.ApplyRoomEffect));
            handlers.Add(Incoming.AddFloorItem, new StaticRequestHandler(SharedPacketLib.PlaceItem));
            handlers.Add(Incoming.PickupItem, new StaticRequestHandler(SharedPacketLib.TakeItem));
            handlers.Add(Incoming.MoveOrRotate, new StaticRequestHandler(SharedPacketLib.MoveItem));
            handlers.Add(Incoming.MoveWall, new StaticRequestHandler(SharedPacketLib.MoveWallItem));
            handlers.Add(Incoming.HandleItem, new StaticRequestHandler(SharedPacketLib.TriggerItem));
            handlers.Add(Incoming.HandleWallItem, new StaticRequestHandler(SharedPacketLib.TriggerItem));
            handlers.Add(Incoming.OpenPostIt, new StaticRequestHandler(SharedPacketLib.OpenPostit));
            handlers.Add(Incoming.SavePostIt, new StaticRequestHandler(SharedPacketLib.SavePostit));
            handlers.Add(Incoming.DeletePostIt, new StaticRequestHandler(SharedPacketLib.DeletePostit));
            handlers.Add(Incoming.OpenGift, new StaticRequestHandler(SharedPacketLib.OpenPresent));
            handlers.Add(Incoming.StartMoodlight, new StaticRequestHandler(SharedPacketLib.GetMoodlight));
            handlers.Add(Incoming.ApplyMoodlightChanges, new StaticRequestHandler(SharedPacketLib.UpdateMoodlight));
            handlers.Add(Incoming.TurnOnMoodlight, new StaticRequestHandler(SharedPacketLib.SwitchMoodlightStatus));
            handlers.Add(Incoming.SendOffer, new StaticRequestHandler(SharedPacketLib.OfferTradeItem));
            handlers.Add(Incoming.CancelOffer, new StaticRequestHandler(SharedPacketLib.TakeBackTradeItem));
            handlers.Add(Incoming.CancelTrade, new StaticRequestHandler(SharedPacketLib.StopTrade));
            handlers.Add(Incoming.AcceptTrade, new StaticRequestHandler(SharedPacketLib.AcceptTrade));
            handlers.Add(Incoming.UnacceptTrade, new StaticRequestHandler(SharedPacketLib.UnacceptTrade));
            handlers.Add(Incoming.ConfirmTrade, new StaticRequestHandler(SharedPacketLib.CompleteTrade));
            handlers.Add(Incoming.SendRespects, new StaticRequestHandler(SharedPacketLib.GiveRespect));
            handlers.Add(Incoming.StartEffect, new StaticRequestHandler(SharedPacketLib.ApplyEffect));
            handlers.Add(Incoming.EnableEffect, new StaticRequestHandler(SharedPacketLib.EnableEffect));
            handlers.Add(Incoming.ThrowDice, new StaticRequestHandler(SharedPacketLib.TriggerItem));
            handlers.Add(Incoming.DiceOff, new StaticRequestHandler(SharedPacketLib.TriggerItemDiceSpecial));
            handlers.Add(Incoming.RedeemExchangeFurni, new StaticRequestHandler(SharedPacketLib.RedeemExchangeFurni));
            handlers.Add(Incoming.PlacePet, new StaticRequestHandler(SharedPacketLib.PlacePet));
            handlers.Add(Incoming.PetInfo, new StaticRequestHandler(SharedPacketLib.GetPetInfo));
            handlers.Add(Incoming.PickupPet, new StaticRequestHandler(SharedPacketLib.PickUpPet));
            handlers.Add(Incoming.RespetPet, new StaticRequestHandler(SharedPacketLib.RespectPet));
            handlers.Add(Incoming.AddPostIt, new StaticRequestHandler(SharedPacketLib.PlacePostIt));
            handlers.Add(Incoming.AddSaddleToPet, new StaticRequestHandler(SharedPacketLib.AddSaddle));
            handlers.Add(Incoming.RemoveSaddle, new StaticRequestHandler(SharedPacketLib.RemoveSaddle));
            handlers.Add(Incoming.MountOnPet, new StaticRequestHandler(SharedPacketLib.Ride));
            handlers.Add(Incoming.SaveWiredEffect, new StaticRequestHandler(SharedPacketLib.SaveWired));
            handlers.Add(Incoming.SaveWiredTrigger, new StaticRequestHandler(SharedPacketLib.SaveWired));
            handlers.Add(Incoming.UserInformation, new StaticRequestHandler(SharedPacketLib.GetUserInfo));
            handlers.Add(Incoming.LoadProfile, new StaticRequestHandler(SharedPacketLib.LoadProfile));
            handlers.Add(Incoming.SerializeClub, new StaticRequestHandler(SharedPacketLib.GetSubscriptionData));
            handlers.Add(Incoming.BadgesInventary, new StaticRequestHandler(SharedPacketLib.GetBadges));
            handlers.Add(Incoming.ApplyBadge, new StaticRequestHandler(SharedPacketLib.UpdateBadges));
            handlers.Add(Incoming.OpenAchievements, new StaticRequestHandler(SharedPacketLib.GetAchievements));
            handlers.Add(Incoming.ChangeLook, new StaticRequestHandler(SharedPacketLib.ChangeLook));
            handlers.Add(Incoming.ChangeMotto, new StaticRequestHandler(SharedPacketLib.ChangeMotto));
            handlers.Add(Incoming.GetWardrobe, new StaticRequestHandler(SharedPacketLib.GetWardrobe));
            handlers.Add(Incoming.SaveWardrobe, new StaticRequestHandler(SharedPacketLib.SaveWardrobe));
            handlers.Add(Incoming.OpenInventory, new StaticRequestHandler(SharedPacketLib.GetInventory));
            handlers.Add(Incoming.PetInventary, new StaticRequestHandler(SharedPacketLib.GetPetsInventory));
            handlers.Add(2340, new StaticRequestHandler(SharedPacketLib.Stream));
            handlers.Add(2011, new StaticRequestHandler(SharedPacketLib.SendStream));
            handlers.Add(1591, new StaticRequestHandler(SharedPacketLib.StreamLike));

            handlers.Add(Incoming.MannequeNameChange, new StaticRequestHandler(SharedPacketLib.MannequeNameChange));
            handlers.Add(Incoming.MannequeFigureChange, new StaticRequestHandler(SharedPacketLib.MannequeFigureChange));

            handlers.Add(Incoming.SetAdParameters, new StaticRequestHandler(SharedPacketLib.SetAdParameters));

            // Load room
            handlers.Add(Incoming.GetGuestRoom, new StaticRequestHandler(SharedPacketLib.GetGuestRoom));
            handlers.Add(Incoming.GetFurnitureAliases, new StaticRequestHandler(SharedPacketLib.GetFurnitureAliases));
            handlers.Add(Incoming.GetRoomEntryData, new StaticRequestHandler(SharedPacketLib.GetRoomEntryData));
            handlers.Add(Incoming.GetRoomAd, new StaticRequestHandler(SharedPacketLib.GetRoomAd));
            handlers.Add(Incoming.GetHabboGroupBadges, new StaticRequestHandler(SharedPacketLib.GetHabboGroupBadges));

            // Door bell
            handlers.Add(Incoming.GoToFlat, new StaticRequestHandler(SharedPacketLib.GoToFlat));

            // Furniture
            handlers.Add(Incoming.EnterOneWayDoor, new StaticRequestHandler(SharedPacketLib.TriggerItem));
            handlers.Add(Incoming.UseWallItem, new StaticRequestHandler(SharedPacketLib.TriggerItem));

            // Groups
            handlers.Add(Incoming.StartGuildPurchase, new StaticRequestHandler(SharedPacketLib.StartGuildPurchase));
            Logging.WriteLine("Logged " + handlers.Count + " packet handler(s)!");
        }
        #endregion
    }
}
