namespace H.Necessaire.CLI.Commands.NuGetVersioning.Models
{
    class NuSpecTree
    {
        public NuSpecNode? Root { get; set; }

        public void IncrementMajorVersion() => IncrementMajorVersion(Root);
        public void IncrementMinorVersion() => IncrementMinorVersion(Root);
        public void IncrementPatchVersion() => IncrementPatchVersion(Root);
        public void IncrementBuildVersion() => IncrementBuildVersion(Root);
        public void SetVersionSuffix(string suffix) => SetVersionSuffix(suffix, Root);

        private void IncrementMajorVersion(NuSpecNode? node = null)
        {
            if (node?.NuSpecInfo?.IsDirtyVersion ?? true)
                return;

            node?.NuSpecInfo?.IncrementMajorVersion();
            foreach (NuSpecNode? depNode in node?.UsedBy ?? new NuSpecNode[0])
                IncrementMajorVersion(depNode);
        }

        private void IncrementMinorVersion(NuSpecNode? node = null)
        {
            if (node?.NuSpecInfo?.IsDirtyVersion ?? true)
                return;

            node?.NuSpecInfo?.IncrementMinorVersion();
            foreach (NuSpecNode? depNode in node?.UsedBy ?? new NuSpecNode[0])
                IncrementMinorVersion(depNode);
        }

        private void IncrementPatchVersion(NuSpecNode? node = null)
        {
            if (node?.NuSpecInfo?.IsDirtyVersion ?? true)
                return;

            node?.NuSpecInfo?.IncrementPatchVersion();
            foreach (NuSpecNode? depNode in node?.UsedBy ?? new NuSpecNode[0])
                IncrementPatchVersion(depNode);
        }

        private void IncrementBuildVersion(NuSpecNode? node = null)
        {
            if (node?.NuSpecInfo?.IsDirtyVersion ?? true)
                return;

            node?.NuSpecInfo?.IncrementBuildVersion();
            foreach (NuSpecNode? depNode in node?.UsedBy ?? new NuSpecNode[0])
                IncrementBuildVersion(depNode);
        }

        private void SetVersionSuffix(string suffix, NuSpecNode? node = null)
        {
            if (node?.NuSpecInfo?.IsDirtyVersion ?? true)
                return;

            node?.NuSpecInfo?.SetVersionSuffix(suffix);
            foreach (NuSpecNode? depNode in node?.UsedBy ?? new NuSpecNode[0])
                SetVersionSuffix(suffix, depNode);
        }

        public override string ToString()
        {
            return Root?.ToString() ?? "[NoRoot]";
        }
    }
}
