﻿using H.Necessaire.Runtime.CLI.Builders;
using System;
using System.Threading.Tasks;

namespace H.Necessaire.Runtime.CLI
{
    public static class WireupExtensions
    {
        private static ImALogger logger = null;
        public static async Task<OperationResult> Run(this ImAnApiWireup wireup, bool askForCommandIfEmpty = false)
        {
            OperationResult result =
                (
                await
                    wireup
                    .DependencyRegistry
                    .Get<CliCommandFactory>()
                    .Run(askForCommandIfEmpty)
                )
                ?? OperationResult.Win()
                ;

            EnsureLogger(wireup.DependencyRegistry);

            if (!result.IsSuccessful)
            {
                await logger.LogError(string.Join(Environment.NewLine, result.FlattenReasons()));
            }

            return result;
        }

        private static void EnsureLogger(ImADependencyProvider dependencyProvider)
        {
            if (logger != null)
                return;

            logger = dependencyProvider.GetLogger("Wireup", "H.Necessaire.Runtime.CLI");
        }
    }
}
