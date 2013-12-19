﻿using System.Parsing.Intellisense;

namespace Dev2 {
    public static class SyntaxTreeFactory {
        public static SyntaxTreeBuilder CreateDatalistTreeBuilder() {
            SyntaxTreeBuilder builder = new SyntaxTreeBuilder();

            builder.RegisterGrammer(new DatalistGrammer());

            return builder;
        }

        public static SyntaxTreeBuilder CreateInfrigistsTreeBuilder() {
            SyntaxTreeBuilder builder = new SyntaxTreeBuilder();

            builder.RegisterGrammer(new StringLiteralGrammer());
            builder.RegisterGrammer(new InfrigisticNumericLiteralGrammer());
            builder.RegisterGrammer(new BooleanLiteralGrammer());
            builder.RegisterGrammer(new InfrigisticsGrammer());

            return builder;
        }

        public static SyntaxTreeBuilder CreateInfrigistsAndDatalistTreeBuilder() {
            SyntaxTreeBuilder builder = CreateInfrigistsTreeBuilder();

            builder.RegisterGrammer(new DatalistGrammer());

            return builder;
        }


        public static SyntaxTreeBuilder CreateActivityDataItemTreeBuilder() {
            return CreateInfrigistsAndDatalistTreeBuilder();
        }
    }
}