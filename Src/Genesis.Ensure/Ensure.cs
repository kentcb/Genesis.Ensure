namespace Genesis.Ensure
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Contains static methods allowing you to ensure that runtime expectations are upheld.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The methods in this class can be used to validate program state. Each method is only included in compiled
    /// code if the <c>ENSURE</c> compiler symbol is defined. This enables you to completely elide these checks
    /// from performance-sensitive code, such as for mobile applications.
    /// </para>
    /// </remarks>
    public static class Ensure
    {
        /// <summary>
        /// Validates that an argument is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Validates that an argument is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNull<T>(T? argumentValue, string argumentName)
            where T : struct
        {
            if (!argumentValue.HasValue)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Validates that an enumerable argument is not <see langword="null"/> and, optionally, that none of its
        /// items are <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <param name="assertContentsNotNull">
        /// <see langword="true"/> if you also want to assert each of the items in the enumerable, otherwise <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="assertContentsNotNull"/> is <see langword="true"/> and <paramref name="argumentValue"/> contains an item that is
        /// <see langword="null"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNull<T>(IEnumerable<T> argumentValue, string argumentName, bool assertContentsNotNull)
        {
            // make sure the enumerable item itself isn't null
            ArgumentNotNull(argumentValue, argumentName);

            if (assertContentsNotNull && !typeof(T).GetTypeInfo().IsValueType)
            {
                // make sure each item in the enumeration isn't null
                foreach (var item in argumentValue)
                {
                    if (item == null)
                    {
                        throw new ArgumentException("An item inside the enumeration was null.", argumentName);
                    }
                }
            }
        }

        /// <summary>
        /// Validates that a generically-typed argument is not <see langword="null"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you need to assert that an argument value is not <see langword="null"/>, but the type of
        /// that argument is generic. As a generic, it could be <see langword="null"/> if either it's a reference type, or if it
        /// is of type <see cref="Nullable{T}"/>. This method deals with either case.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">
        /// The type of the argument.
        /// </typeparam>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void GenericArgumentNotNull<T>(T argumentValue, string argumentName)
        {
            var typeInfo = typeof(T).GetTypeInfo();

            if (!typeInfo.IsValueType || (typeInfo.IsGenericType && (typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))))
            {
                ArgumentNotNull((object)argumentValue, argumentName);
            }
        }

        /// <summary>
        /// Validates that a <see cref="string"/> argument is not <see langword="null"/> or an empty string.
        /// </summary>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="argumentValue"/> is an empty string.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            Ensure.ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue == "")
            {
                throw new ArgumentException("Cannot be empty.", argumentName);
            }
        }

        /// <summary>
        /// Validates that an <see cref="IEnumerable"/> argument is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="argumentValue"/> is empty.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNullOrEmpty(IEnumerable argumentValue, string argumentName)
        {
            Ensure.ArgumentNotNull(argumentValue, argumentName);

            if (!argumentValue.GetEnumerator().MoveNext())
            {
                throw new ArgumentException("Cannot be empty.", argumentName);
            }
        }

        /// <summary>
        /// Validates that an <see cref="ICollection"/> argument is not <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="argumentValue"/> is empty.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNullOrEmpty(ICollection argumentValue, string argumentName)
        {
            Ensure.ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.Count == 0)
            {
                throw new ArgumentException("Cannot be empty.", argumentName);
            }
        }

        /// <summary>
        /// Validates that a <see cref="string"/> argument is not <see langword="null"/> or whitespace.
        /// </summary>
        /// <param name="argumentValue">
        /// The argument value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argumentValue"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="argumentValue"/> contains only whitespace (or is empty).
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentNotNullOrWhiteSpace(string argumentValue, string argumentName)
        {
            Ensure.ArgumentNotNull(argumentValue, argumentName);

            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException("Cannot be white-space.", argumentName);
            }
        }

        /// <summary>
        /// Validates that a general condition holds true for an argument.
        /// </summary>
        /// <param name="condition">
        /// The condition that must be <see langword="true"/>.
        /// </param>
        /// <param name="message">
        /// The message for the exception if the condition fails to hold.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="condition"/> is <see langword="false"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentCondition(bool condition, string message, string argumentName)
        {
            if (!condition)
            {
                throw new ArgumentException(message, argumentName);
            }
        }

        /// <summary>
        /// Validates that a value is a valid member of an enumeration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will throw if <paramref name="enumerationValue"/> is not a member of <typeparamref name="TEnum"/>.
        /// It works equally well for flags enumerations, in which case it will ensure that <paramref name="enumerationValue"/>
        /// is a valid combination of flags defined by <typeparamref name="TEnum"/>.
        /// </para>
        /// <para>
        /// The valid values for the enumeration are inferred automatically as the set of all enumeration values in
        /// <typeparamref name="TEnum"/>. In certain scenarios you may wish to restrict this to specific enumeration
        /// members, in which case you can use the overload that takes a set of valid values.
        /// </para>
        /// </remarks>
        /// <typeparam name="TEnum">
        /// The enumeration type.
        /// </typeparam>
        /// <param name="enumerationValue">
        /// The enumeration value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="enumerationValue"/> is not a valid member of <typeparamref name="TEnum"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentIsValidEnum<TEnum>(TEnum enumerationValue, string argumentName)
            where TEnum : struct
        {
            var validValues = Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToArray();

            Ensure.ArgumentIsValidEnum(
                enumerationValue,
                argumentName,
                validValues);
        }

        /// <summary>
        /// Validates that a value is a valid member of an enumeration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method will throw if <paramref name="enumerationValue"/> is not a member of <typeparamref name="TEnum"/>.
        /// It works equally well for flags enumerations, in which case it will ensure that <paramref name="enumerationValue"/>
        /// is a valid combination of flags defined by <typeparamref name="TEnum"/>.
        /// </para>
        /// <para>
        /// This overload allows you to specify the valid values explicitly. This can be useful if only a subset of values
        /// are valid.
        /// </para>
        /// </remarks>
        /// <typeparam name="TEnum">
        /// The enumeration type.
        /// </typeparam>
        /// <param name="enumerationValue">
        /// The enumeration value.
        /// </param>
        /// <param name="argumentName">
        /// The argument name.
        /// </param>
        /// <param name="validValues">
        /// The values that should be considered valid.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="enumerationValue"/> is not a valid member of <typeparamref name="TEnum"/>.
        /// </exception>
        [Conditional("ENSURE")]
        public static void ArgumentIsValidEnum<TEnum>(TEnum enumerationValue, string argumentName, params TEnum[] validValues)
            where TEnum : struct
        {
            ArgumentNotNull(validValues, "validValues");

            if (typeof(TEnum).GetTypeInfo().GetCustomAttribute<FlagsAttribute>(false) != null)
            {
                // flag enumeration
                bool throwEx;
                var longValue = Convert.ToInt64(enumerationValue, CultureInfo.InvariantCulture);

                if (longValue == 0)
                {
                    // only throw if zero isn't permitted by the valid values
                    throwEx = true;

                    foreach (TEnum value in validValues)
                    {
                        if (Convert.ToInt64(value, CultureInfo.InvariantCulture) == 0)
                        {
                            throwEx = false;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var value in validValues)
                    {
                        longValue &= ~Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    }

                    // throw if there is a value left over after removing all valid values
                    throwEx = longValue != 0;
                }

                if (throwEx)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not valid for flags enumeration '{1}'.",
                            enumerationValue,
                            typeof(TEnum).FullName),
                        argumentName);
                }
            }
            else
            {
                // not a flag enumeration
                foreach (var value in validValues)
                {
                    if (enumerationValue.Equals(value))
                    {
                        return;
                    }
                }

                // at this point we know an exception is required - however, we want to tailor the message based on whether the
                // specified value is undefined or simply not allowed
                if (!Enum.IsDefined(typeof(TEnum), enumerationValue))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is not defined for enumeration '{1}'.",
                            enumerationValue,
                            typeof(TEnum).FullName),
                        argumentName);
                }
                else
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Enum value '{0}' is defined for enumeration '{1}' but it is not permitted in this context.",
                            enumerationValue,
                            typeof(TEnum).FullName),
                        argumentName);
                }
            }
        }

        /// <summary>
        /// Validates that a general condition holds true.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is a very general purpose, catch-all means of validating some condition. Use it only if all other
        /// options have been exhausted.
        /// </para>
        /// </remarks>
        /// <param name="condition">
        /// The condition that must be <see langword="true"/>.
        /// </param>
        /// <param name="getException">
        /// Gets the exception to throw if <paramref name="condition"/> fails to hold.
        /// </param>
        [Conditional("ENSURE")]
        public static void Condition(bool condition, Func<Exception> getException)
        {
            if (!condition)
            {
                throw getException();
            }
        }
    }
}