﻿namespace MangAppClient.Core.Model
{
    using MangAppClient.Core.Model;

    public sealed class UpdateDiffResult : DiffResult
    {
        internal UpdateDiffResult(int id, int lastChapter, MangaStatus? newStatus = null) :
            base(id)
        {
            this.LastChapter = lastChapter;
            this.NewStatus = newStatus;
        }

        internal int LastChapter { get; private set; }

        internal MangaStatus? NewStatus { get; private set; }
    }
}