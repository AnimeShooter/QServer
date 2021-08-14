using System;
using System.Collections.Generic;
using System.Text;

namespace Qserver.GameServer.Qpang
{
    public class PlayerEffectManager
    {
        private uint _lastTick;
        private List<Effect> _effects;
        private RoomSessionPlayer _player;

        public PlayerEffectManager(RoomSessionPlayer player)
        {
            this._effects = new List<Effect>();
            this._player = player;
            this._lastTick = Util.Util.Timestamp();
        }

        public void Tick()
        {
            var currTime = Util.Util.Timestamp();

            if (currTime <= this._lastTick)
                return;

            this._lastTick = currTime;

            if (this._effects.Count == 0)
                return;

            List<Effect> expired = new List<Effect>();
            for(int i = 0; i < this._effects.Count; i++)
            {
                var effect = this._effects[i];
                effect.Weapon.EffectDuration--;
                if (effect.Weapon.EffectId == 12 || effect.Weapon.EffectId == 13) // pois || fire
                    TakeDamageFromEffect(effect);
                if (effect.Weapon.EffectDuration <= 0)
                    expired.Add(effect);
            }

            foreach(var effect in expired)
            {
                RemoveEffect(effect.Weapon.EffectId);
                this._effects.Remove(effect);
            }
        }

        public void AddEffect(RoomSessionPlayer owner, Weapon weapon, uint entityId)
        {
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                var effect = new Effect()
                {
                    EntityId = entityId,
                    Weapon = weapon,
                    Target = owner
                };
                this._effects.Add(effect);
            }
        }

        public void Clear()
        {
            if (this._player == null)
                return;

            List<byte> clearedEffects = new List<byte>();

            lock(this._player.Lock)
            {
                foreach(var effect in this._effects)
                {
                    // NOTE: maybe!
                    if (clearedEffects.Contains(effect.Weapon.EffectId))
                        continue;

                    // TOD)
                    clearedEffects.Add(effect.Weapon.EffectId);
                    //this._player.Post(new GCWeapon(this._player.Player.PlayerId, 6, effect.Weapon.EffectId));

                }
            }

            this._effects.Clear();
        }


        private void RemoveEffect(byte effectId)
        {
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                uint sharedEffectCount = 0;

                foreach (var effect in this._effects)
                    if (effect.Weapon.EffectId == effectId && effect.Weapon.EffectId > 0)
                        sharedEffectCount++;
                // TODO
                //if (sharedEffectCount == 0)
                //    this._player.RoomSession.RelayPlaying<GCWeapon>(this._player.Player.PlayerId, 6, effectId);
            }
        }

        private void TakeDamageFromEffect(Effect effect)
        {
            if (this._player == null)
                return;

            lock (this._player.Lock)
            {
                this._player.TakeHealth(5); // all dmg effects areset to 5

                var owner = effect.Target;
                if (owner == null)
                    return;

                lock(owner.Lock)
                {
                    //this._player.RoomSession.RelayPlaying<GCHit>(owner.Player.PlayerId, this._player.Player.PlayerId, 1, 0, 0, 0, 0, 0, 0, effect.EntityId, 1, 1,
                    //    this._player.Health, 5, effect.Weapon.ItemId, 1, 1, 1, owner.Streak + 1, 0, 0);

                    if(this._player.Death)
                    {
                        owner.RoomSession.GameMode.OnPlayerKill(owner, this._player, effect.Weapon, 1);
                        owner.RoomSession.RelayPlaying<GCGameState>(this._player.Player.PlayerId, (uint)17, (uint)effect.Weapon.EffectId, owner.Player.PlayerId);
                    }
                }
            }
        }

    }
}
