// Copyright (c) 2021 DrBarnabus

using Figgle;
using Microsoft.Extensions.DependencyInjection;
using Servly.Core.Options;
using System;
using System.Runtime.InteropServices;

namespace Servly.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string MainConfigurationSection = "Servly";

        public static IServiceCollection AddServly(this IServiceCollection services, Action<IServlyBuilder>? configureDelegate = null)
        {
            var servlyBuilder = ServlyBuilder.Create(services);

            servlyBuilder.AddOptions<ServlyOptions>(MainConfigurationSection);

            configureDelegate?.Invoke(servlyBuilder);

            DisplayStartupInformation(servlyBuilder);

            servlyBuilder.Build();

            return services;
        }

        private static void DisplayStartupInformation(IServlyBuilder servlyBuilder)
        {
            var servlyOptions = servlyBuilder.GetOptions<ServlyOptions>();

            string fullWidthSeparatorString = new('=', Console.BufferWidth);

            if (servlyOptions.DisplayStartupBanner && !string.IsNullOrEmpty(servlyOptions.ServiceName))
            {
                Console.WriteLine(fullWidthSeparatorString);
                Console.WriteLine(FiggleFonts.Doom.Render(servlyOptions.ServiceName));
                Console.Write(fullWidthSeparatorString);
            }

            if (servlyOptions.DisplayPlatformInformation)
            {
                if (!servlyOptions.DisplayStartupBanner)
                    Console.Write(fullWidthSeparatorString);

                Console.WriteLine("Platform Information:");
                Console.WriteLine("    .NET Runtime Version: {0}", RuntimeInformation.FrameworkDescription);
                Console.WriteLine("    Processor Architecture: {0}", RuntimeInformation.ProcessArchitecture);
                Console.WriteLine("    Operating System: {0} ({1})", RuntimeInformation.OSDescription, RuntimeInformation.OSArchitecture);

                // TODO: Add Servly Version once Versioned Builds are Setup

                Console.Write(fullWidthSeparatorString);
            }
        }
    }
}
