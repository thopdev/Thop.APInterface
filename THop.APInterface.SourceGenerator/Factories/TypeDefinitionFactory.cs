﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using THop.APInterface.SourceGenerator.ClassGenerators;
using THop.APInterface.SourceGenerator.Factories.Interfaces;
using THop.APInterface.SourceGenerator.Models.Definitions.TypeDefinitions;

namespace THop.APInterface.SourceGenerator.Factories
{
    public class TypeDefinitionFactory : ITypeDefinitionFactory
    {
        private readonly IAttributeDefinitionFactory _attributeDefinitionFactory;
        private readonly IMethodDefinitionFactory _methodDefinitionFactory;

        public TypeDefinitionFactory(IAttributeDefinitionFactory attributeDefinitionFactory,
            IMethodDefinitionFactory methodDefinitionFactory)
        {
            _attributeDefinitionFactory = attributeDefinitionFactory;
            _methodDefinitionFactory = methodDefinitionFactory;
        }

        public TypeDefinition CreateTypeDefinitionFromSyntax(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax switch
            {
                InterfaceDeclarationSyntax interfaceDeclarationSyntax => InterfaceDefinitionFromSyntax(interfaceDeclarationSyntax),

                ClassDeclarationSyntax _ => throw new NotImplementedException(
                    "ClassDeclaration is not yet implemented"),

                _ => throw new NotImplementedException(
                    $"{typeDeclarationSyntax.GetType()} is not yet implemented for CreateTypeDefinitionFromSyntax")
            };
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private InterfaceDefinition InterfaceDefinitionFromSyntax(InterfaceDeclarationSyntax interfaceDeclarationSyntax)
        {
            var typeName = interfaceDeclarationSyntax.Identifier.ValueText;
            var attributes = interfaceDeclarationSyntax.AttributeLists.FirstOrDefault()?.Attributes
                .Select(_attributeDefinitionFactory.CreateAttributeFromSyntax).ToArray() ?? new AttributeDefinition[0];

            var members = interfaceDeclarationSyntax.Members.OfType<MethodDeclarationSyntax>()
                .Select(member => _methodDefinitionFactory.CreateMethodFromSyntax(member)).ToArray();

            return new InterfaceDefinition(typeName, attributes, new string[0], members);
        }
    }
}