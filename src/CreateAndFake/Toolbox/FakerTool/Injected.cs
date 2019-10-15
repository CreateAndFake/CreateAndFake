﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateAndFake.Toolbox.FakerTool
{
    /// <summary>Instance with dynamically injected fakes.</summary>
    /// <typeparam name="T">Type being injected by fakes.</typeparam>
    public sealed class Injected<T>
    {
        /// <summary>Faked implementation.</summary>
        public T Dummy { get; }

        /// <summary>Fakes injected into the dummy.</summary>
        public IEnumerable<Fake> Fakes { get; }

        /// <summary>Initializer.</summary>
        /// <param name="dummy">Faked implementation.</param>
        /// <param name="fakes">Fakes injected into the dummy.</param>
        public Injected(T dummy, IEnumerable<Fake> fakes)
        {
            Dummy = dummy ?? throw new ArgumentNullException(nameof(dummy));
            Fakes = fakes?.ToArray() ?? throw new ArgumentNullException(nameof(fakes));
        }

        /// <summary>Finds the fake of the given type.</summary>
        /// <typeparam name="TInject">Type of fake to look for.</typeparam>
        /// <param name="skip">How many fakes of the given type to ignore first.</param>
        /// <returns>The found fake.</returns>
        public Fake<TInject> Fake<TInject>(int skip = 0)
        {
            Fake<TInject> direct = Fakes
                .OfType<Fake<TInject>>()
                .Skip(skip)
                .FirstOrDefault();

            return direct ?? Fakes
                .Where(f => f.Dummy.GetType().Inherits<TInject>())
                .Skip(skip)
                .First()
                .ToFake<TInject>();
        }

        /// <summary>Finds the fake owner of the given dummy.</summary>
        /// <typeparam name="TInject">Type of fake to look for.</typeparam>
        /// <param name="dummy">Dummy of a fake to search on.</param>
        /// <returns>The found fake.</returns>
        public Fake<TInject> Fake<TInject>(TInject dummy)
        {
            return Fakes.First(f => ReferenceEquals(f.Dummy, dummy)).ToFake<TInject>();
        }

        /// <summary>Verifies all behaviors in all fakes were called as expected.</summary>
        public void VerifyAll()
        {
            foreach (Fake fake in Fakes)
            {
                fake.VerifyAll();
            }
        }
    }
}
