namespace Metalama.Samples.Transactional;

public abstract partial class TransactionalObject
{
    protected abstract class TransactionalObjectState : ITransactionalObjectState
    {
        private readonly TransactionalObjectId _objectId;
        private volatile int _status = (int) TransactionalObjectStateStatus.Editable;

        protected TransactionalObjectState( TransactionalObjectId objectId )
        {
            this._objectId = objectId;
        }

        TransactionalObjectStateStatus ITransactionalObjectState.Status =>
            (TransactionalObjectStateStatus) this._status;

        public void MakeReadOnly()
        {
            var previousStatus = (TransactionalObjectStateStatus) Interlocked.CompareExchange(
                ref this._status,
                (int) TransactionalObjectStateStatus.ReadOnly,
                (int) TransactionalObjectStateStatus.Editable );

            if ( previousStatus != TransactionalObjectStateStatus.Editable )
            {
                throw new InvalidOperationException(
                    $"Cannot call ToReadOnly when the status is '{previousStatus}'." );
            }
        }

        private void CheckStatus( TransactionalObjectStateStatus requiredStatus )
        {
            var status = (TransactionalObjectStateStatus) this._status;

            if ( this._status != (int) requiredStatus )
            {
                // We avoid calling ToString on the originator.
                throw new InvalidOperationException(
                    $"The status of {this._objectId} was expected to be {requiredStatus} but was {status}." );
            }
        }

        private void CheckStatus( TransactionalObjectStateStatus requiredStatus1,
            TransactionalObjectStateStatus requiredStatus2 )
        {
            var status = (TransactionalObjectStateStatus) this._status;

            if ( this._status != (int) requiredStatus1 && this._status != (int) requiredStatus2 )
            {
                // We avoid calling ToString on the originator.
                throw new InvalidOperationException(
                    $"The status of the {this._objectId} was expected to be {requiredStatus1} or {requiredStatus2} but was {status}." );
            }
        }

        protected void CheckCanEdit() =>
            this.CheckStatus( TransactionalObjectStateStatus.Editable );

        protected virtual TransactionalObjectState Clone() =>
            (TransactionalObjectState) this.MemberwiseClone();

        public virtual ITransactionalObjectState ToEditable()
        {
            this.CheckStatus( TransactionalObjectStateStatus.ReadOnly );

            var clone = this.Clone();
            clone._status = (int) TransactionalObjectStateStatus.Editable;

            return clone;
        }

        public virtual ITransactionalObjectState ToDeleted()
        {
            this.CheckStatus( TransactionalObjectStateStatus.ReadOnly,
                TransactionalObjectStateStatus.Editable );

            var clone = this.Clone();
            clone._status = (int) TransactionalObjectStateStatus.Deleted;

            return clone;
        }
    }
}