using Irony.Parsing;

namespace Irony.Samples.Java
{
  partial class JavaGrammar
  {
    private void InitializeSyntax()
    {
      bool enableAutomaticConflictResolution = true; //Roman: moved it here and made var instead of const to get rid of compiler warnings

#region Identifier and Literals
      #pragma warning disable 168
      #pragma warning disable 162
      // ReSharper disable InconsistentNaming
      // ReSharper disable ConditionIsAlwaysTrueOrFalse

      var identifier_raw = new IdentifierTerminal("_identifier_")
      {
        AllFirstChars = ValidIdentifierStartCharacters,
        AllChars = ValidIdentifierCharacters
      };
      var identifier = new NonTerminal("identifier")
                        {
                          Rule = enableAutomaticConflictResolution
                                  ? PreferShiftHere() + identifier_raw
                                  : identifier_raw
                        };

      var number_literal = CreateJavaNumber("number");
      var character_literal = CreateJavaChar("char");
      var string_literal = CreateJavaString("string");
      var null_literal = CreateJavaNull("null");
      
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      // ReSharper restore InconsistentNaming
      #pragma warning restore 162
      #pragma warning restore 168
#endregion

#region Terminals
      #pragma warning disable 168
      // ReSharper disable InconsistentNaming

      var ABSTRACT = ToTerm("abstract", "abstract");
      var AMP = ToTerm("&", "amp");
      var AMP_AMP = ToTerm("&&", "amp_amp");
      var AMP_ASSIGN = ToTerm("&=", "amp_assign");
      var ASSERT = ToTerm("assert", "assert");
      var ASSIGN = ToTerm("=", "assign");
      var AT = ToTerm("@", "at");
      var BAR = ToTerm("|", "bar");
      var BAR_ASSIGN = ToTerm("|=", "bar_assign");
      var BAR_BAR = ToTerm("||", "bar_bar");
      var BOOLEAN = ToTerm("boolean", "boolean");
      var BREAK = ToTerm("break", "break");
      var BYTE = ToTerm("byte", "byte");
      var CARET = ToTerm("^", "caret");
      var CARET_ASSIGN = ToTerm("^=", "caret_assign");
      var CASE = ToTerm("case", "case");
      var CATCH = ToTerm("catch", "catch");
      var CHAR = ToTerm("char", "char");
      var CLASS_TOKEN = ToTerm("class", "class_token");
      var COLON = ToTerm(":", "colon");
      var COMMA = ToTerm(",", "comma");
      var CONST = ToTerm("const", "const");
      var CONTINUE = ToTerm("continue", "continue");
      var DEFAULT = ToTerm("default", "default");
      var DO = ToTerm("do", "do");
      var DOT_DOT_DOT = ToTerm("...", "dot_dot_dot");
      var DOUBLE = ToTerm("double", "double");
      var ELSE = ToTerm("else", "else");
      var EMARK = ToTerm("!", "emark");
      var ENUM = ToTerm("enum", "enum");
      var EQ = ToTerm("==", "eq");
      var EXTENDS = ToTerm("extends", "extends");
      var FALSE = ToTerm("false", "false");
      var FINAL = ToTerm("final", "final");
      var FINALLY_TOKEN = ToTerm("finally", "finally_token");
      var FLOAT = ToTerm("float", "float");
      var FOR = ToTerm("for", "for");
      var GOTO = ToTerm("goto", "goto");
      var GT = ToTerm(">", "gt");
      var GTEQ = ToTerm(">=", "gteq");
      var IF = ToTerm("if", "if");
      var IMPLEMENTS = ToTerm("implements", "implements");
      var IMPORT = ToTerm("import", "import");
      var INSTANCEOF = ToTerm("instanceof", "instanceof");
      var INT = ToTerm("int", "int");
      var INTERFACE = ToTerm("interface", "interface");
      var L_BRC = ToTerm("{", "l_brc");
      var LONG = ToTerm("long", "long");
      var LTEQ = ToTerm("<=", "lteq");
      var MINUS = ToTerm("-", "minus");
      var MINUS_ASSIGN = ToTerm("-=", "minus_assign");
      var MINUS_MINUS = ToTerm("--", "minus_minus");
      var NATIVE = ToTerm("native", "native");
      var NEQ = ToTerm("!=", "neq");
      var NEW = ToTerm("new", "new");
      var PACKAGE = ToTerm("package", "package");
      var PERCENT = ToTerm("%", "percent");
      var PERCENT_ASSIGN = ToTerm("%=", "percent_assign");
      var PLUS = ToTerm("+", "plus");
      var PLUS_ASSIGN = ToTerm("+=", "plus_assign");
      var PLUS_PLUS = ToTerm("++", "plus_plus");
      var PRIVATE = ToTerm("private", "private");
      var PROTECTED = ToTerm("protected", "protected");
      var PUBLIC = ToTerm("public", "public");
      var QMARK = ToTerm("?", "qmark");
      var R_BKT = ToTerm("]", "r_bkt");
      var R_BRC = ToTerm("}", "r_brc");
      var R_PAR = ToTerm(")", "r_par");
      var RETURN = ToTerm("return", "return");
      var SEMI = ToTerm(";", "semi");
      var SHL = ToTerm("<<", "shl");
      var SHL_ASSIGN = ToTerm("<<=", "shl_assign");
      var SHORT = ToTerm("short", "short");
      var SHR = ToTerm(">>", "shr");
      var SHR_ASSIGN = ToTerm(">>=", "shr_assign");
      var SLASH = ToTerm("/", "slash");
      var SLASH_ASSIGN = ToTerm("/=", "slash_assign");
      var STAR = ToTerm("*", "star");
      var STAR_ASSIGN = ToTerm("*=", "star_assign");
      var STATIC = ToTerm("static", "static");
      var STRICTFP = ToTerm("strictfp", "strictfp");
      var SWITCH = ToTerm("switch", "switch");
      var SYNCHRONIZED = ToTerm("synchronized", "synchronized");
      var THROW = ToTerm("throw", "throw");
      var THROWS_TOKEN = ToTerm("throws", "throws_token");
      var TILDE = ToTerm("~", "tilde");
      var TRANSIENT = ToTerm("transient", "transient");
      var TRUE = ToTerm("true", "true");
      var TRY = ToTerm("try", "try");
      var USHR = ToTerm(">>>", "ushr");
      var USHR_ASSIGN = ToTerm(">>>=", "ushr_assign");
      var VOID = ToTerm("void", "void");
      var VOLATILE = ToTerm("volatile", "volatile");
      var WHILE = ToTerm("while", "while");

      #region Terminals with conflicts
      #pragma warning disable 162
      // ReSharper disable ConditionIsAlwaysTrueOrFalse

      var THIS_RAW = ToTerm("this", "this");
      var THIS = new NonTerminal("_this_")
                  {
                    Rule = enableAutomaticConflictResolution
                            ? PreferShiftHere() + THIS_RAW
                            : THIS_RAW
                  };

      var LT_RAW = ToTerm("<", "lt");
      var LT = new NonTerminal("_<_")
                {
                  Rule = CustomActionHere(ResolveConflicts) + LT_RAW
                };
      
      var L_PAR_RAW = ToTerm("(", "l_par");
      var L_PAR = new NonTerminal("_(_")
                    {
                      Rule = enableAutomaticConflictResolution
                              ? PreferShiftHere() + L_PAR_RAW
                              : L_PAR_RAW
                    };
      
      var L_BKT_RAW = ToTerm("[", "l_bkt");
      var L_BKT = new NonTerminal("_[_")
                    {
                      Rule = enableAutomaticConflictResolution
                              ? PreferShiftHere() + L_BKT_RAW
                              : L_BKT_RAW
                    };
      
      var DOT_RAW = ToTerm(".", "dot");
      var DOT = new NonTerminal("_._")
      {
        Rule = CustomActionHere(ResolveConflicts) + DOT_RAW
      };
      var SUPER_TOKEN_RAW = ToTerm("super", "super_token");
      var SUPER_TOKEN = new NonTerminal("_super_")
      {
        Rule = CustomActionHere(ResolveConflicts) + SUPER_TOKEN_RAW
      };
      
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      #pragma warning restore 162
      #endregion
      
      // ReSharper restore InconsistentNaming
      #pragma warning restore 168
#endregion

#region NonTerminal Declarations
      // ReSharper disable InconsistentNaming
      var abstract_method_declaration = new NonTerminal("abstract_method_declaration");
      var annotation = new NonTerminal("annotation");
      var annotation_declaration = new NonTerminal("annotation_declaration");
      var annotation_type_body = new NonTerminal("annotation_type_body");
      var annotation_type_element_declaration = new NonTerminal("annotation_type_element_declaration");
      var annotation_type_element_rest = new NonTerminal("annotation_type_element_rest");
      var annotations = new NonTerminal("annotations");
      var argument_list = new NonTerminal("argument_list");
      var arguments = new NonTerminal("arguments");
      var arguments_opt = new NonTerminal("arguments?");
      var array_access = new NonTerminal("array_access");
      var array_creator_rest = new NonTerminal("array_creator_rest");
      var array_initializer = new NonTerminal("array_initializer");
      var assignment_expression = new NonTerminal("assignment_expression");
      var assignment_operator = new NonTerminal("assignment_operator");
      var base_type_declaration = new NonTerminal("type_declaration_without_modifiers");
      var binary_expression = new NonTerminal("binary_expression");
      var block = new NonTerminal("block");
      var block_statement = new NonTerminal("block_statement");
      var block_statements = new NonTerminal("block_statements");
      var boolean_literal = new NonTerminal("boolean_literal");
      var cast_expression = new NonTerminal("cast_expression");
      var catch_clause = new NonTerminal("catch_clause");
      var catches = new NonTerminal("catches");
      var class_body = new NonTerminal("class_body");
      var class_body_declaration = new NonTerminal("class_body_declaration");
      var class_body_opt = new NonTerminal("class_body_opt");
      var class_creator_rest = new NonTerminal("class_creator_rest");
      var class_declaration = new NonTerminal("class_declaration");
      var class_member_declaration = new NonTerminal("class_member_declaration");
      var compilation_unit = new NonTerminal("compilation_unit");
      var constant_declaration = new NonTerminal("constant_declaration");
      var constructor_body = new NonTerminal("constructor_body");
      var constructor_declaration = new NonTerminal("constructor_declaration");
      var created_name = new NonTerminal("created_name");
      var creator = new NonTerminal("creator");
      var dim = new NonTerminal("dim");
      var dim_expr = new NonTerminal("dim_expr");
      var dim_exprs = new NonTerminal("dims_exprs");
      var dims = new NonTerminal("dims");
      var element_value = new NonTerminal("element_value");
      var element_value_array_initializer = new NonTerminal("element_value_array_initializer");
      var element_value_pair = new NonTerminal("element_value_pair");
      var element_value_pairs = new NonTerminal("element_value_pairs");
      var element_values = new NonTerminal("element_values");
      var enum_body = new NonTerminal("enum_body");
      var enum_body_declarations = new NonTerminal("enum_body_declarations");
      var enum_body_declarations_opt = new NonTerminal("enum_body_declarations?");
      var enum_constant = new NonTerminal("enum_constant");
      var enum_constants = new NonTerminal("enum_constants");
      var enum_declaration = new NonTerminal("enum_declaration");
      var exception_type_list = new NonTerminal("exception_type_list");
      var explicit_constructor_invocation = new NonTerminal("explicit_constructor_invocation");
      var explicit_generic_invocation = new NonTerminal("explicit_generic_invocation");
      var explicit_generic_invocation_suffix = new NonTerminal("explicit_generic_invocation_suffix");
      var expression = new NonTerminal("expression");
      var expression_in_parens = new NonTerminal("expression_in_parens");
      var field_access = new NonTerminal("field_access");
      var field_declaration = new NonTerminal("field_declaration");
      var for_control = new NonTerminal("for_control");
      var for_init = new NonTerminal("for_init");
      var for_update = new NonTerminal("for_update");
      var for_var_control = new NonTerminal("for_var_control");
      var formal_parameter = new NonTerminal("formal_parameter");
      var formal_parameter_list = new NonTerminal("formal_parameter_list");
      var formal_parameter_list_opt = new NonTerminal("formal_parameter_list?");
      var formal_parameters = new NonTerminal("formal_parameters");
      var identifier_suffix = new NonTerminal("identifier_suffix");
      var identifier_suffix_opt = new NonTerminal("identifier_suffix_opt");
      var import_declaration = new NonTerminal("import_declaration");
      var import_declarations = new NonTerminal("import_declarations");
      var import_wildcard = new NonTerminal("import_wildcard");
      var infix_operator = new NonTerminal("infix_operator");
      var inner_creator = new NonTerminal("inner_creator");
      var instance_initializer = new NonTerminal("instance_initializer");
      var interface_body = new NonTerminal("interface_body");
      var interface_declaration = new NonTerminal("interface_declaration");
      var interface_member_declaration = new NonTerminal("interface_member_declaration");
      var interface_type = new NonTerminal("interface_type");
      var interface_type_list = new NonTerminal("interface_type_list");
      var interfaces = new NonTerminal("interfaces");
      var interfaces_opt = new NonTerminal("interfaces?");
      var last_formal_parameter = new NonTerminal("last_formal_parameter");
      var left_hand_side = new NonTerminal("left_hand_side");
      var literal = new NonTerminal("literal");
      var local_variable_declaration = new NonTerminal("local_variable_declaration_statement");
      var method_body = new NonTerminal("method_body");
      var method_declaration = new NonTerminal("method_declaration");
      var method_declarator = new NonTerminal("method_declarator");
      var method_invocation = new NonTerminal("method_invocation");
      var modifier = new NonTerminal("modifier");
      var modifiers = new NonTerminal("modifiers");
      var modifiers_opt = new NonTerminal("modifiers?");
      var normal_import_declaration = new NonTerminal("normal_import_declaration");
      var package_declaration = new NonTerminal("package_declaration");
      var package_declaration_w_modifiers = new NonTerminal("package_declaration");
      var postfix_operator = new NonTerminal("postfix_operator");
      var prefix_operator = new NonTerminal("prefix_operator");
      var primary_expression = new NonTerminal("primary_expression");
      var primary_expression_no_new = new NonTerminal("primary_expression_no_new");
      var primitive_type = new NonTerminal("primitive_type");
      var qualified_name = new NonTerminal("qualified_name");
      var selector = new NonTerminal("selector");
      var statement = new NonTerminal("statement");
      var statement_expression = new NonTerminal("statement_expression");
      var static_import_declaration = new NonTerminal("static_import_declaration");
      var static_initializer = new NonTerminal("static_initializer");
      var super = new NonTerminal("super");
      var super_opt = new NonTerminal("super?");
      var super_suffix = new NonTerminal("super_suffix");
      var switch_block_statement_group = new NonTerminal("switch_block_statement_group");
      var switch_block_statement_groups = new NonTerminal("switch_block_statement_groups");
      var switch_label = new NonTerminal("switch_label");
      var templated_identifier = new NonTerminal("templated_identifier");
      var templated_qualified_name = new NonTerminal("templated_qualified_name");
      var throws = new NonTerminal("throws");
      var throws_opt = new NonTerminal("throws?");
      var trinary_expression = new NonTerminal("trinary_expression");
      var type = new NonTerminal("type");
      var type_argument = new NonTerminal("type_argument");
      var type_argument_list = new NonTerminal("type_argument_list");
      var type_arguments = new NonTerminal("type_arguments");
      var type_arguments_opt = new NonTerminal("type_arguments?");
      var type_bound = new NonTerminal("type_bound");
      var type_bound_list = new NonTerminal("type_bound_list");
      var type_bound_opt = new NonTerminal("type_bound?");
      var type_declaration = new NonTerminal("type_declaration");
      var type_declaration_w_modifiers = new NonTerminal("type_declaration_with_modifiers");
      var type_declarations = new NonTerminal("type_declarations");
      var type_parameter = new NonTerminal("type_parameter");
      var type_parameter_list = new NonTerminal("type_parameter_list");
      var type_parameters = new NonTerminal("type_parameters");
      var type_parameters_opt = new NonTerminal("type_parameters?");
      var unary_expression = new NonTerminal("unary_expression");
      var variable_declarator = new NonTerminal("variable_declarator");
      var variable_declarators = new NonTerminal("variable_declarators");
      var variable_declarators_rest = new NonTerminal("variable_declarators_rest");
      var variable_initializer = new NonTerminal("variable_initializer");
      var variable_initializers = new NonTerminal("variable_initializers");
      // ReSharper restore InconsistentNaming
#endregion

#region NonTerminal Rules

      #region common
      modifiers_opt.Rule = Empty | modifiers;
      modifiers.Rule = MakePlusRule(modifiers, modifier);
      modifier.Rule = ABSTRACT | FINAL | NATIVE | PRIVATE | PROTECTED | PUBLIC | STATIC | STRICTFP | SYNCHRONIZED | TRANSIENT | VOLATILE | annotation;
      qualified_name.Rule = MakePlusRule(qualified_name, DOT, identifier);
      type.Rule = templated_qualified_name;
      type.Rule |= templated_qualified_name + dim + dims;
      type.Rule |= primitive_type;
      type.Rule |= primitive_type + dim + dims;

      super_opt.Rule = Empty | super;
      super.Rule = EXTENDS + templated_qualified_name;

      interfaces_opt.Rule = Empty | interfaces;
      interfaces.Rule = IMPLEMENTS + interface_type_list;
      interface_type_list.Rule = MakePlusRule(interface_type_list, COMMA, interface_type);
      interface_type.Rule = templated_qualified_name;

      templated_identifier.Rule = identifier + type_arguments;
      templated_identifier.Rule |= identifier;
      templated_qualified_name.Rule = MakePlusRule(templated_qualified_name, DOT, templated_identifier);

      type_arguments_opt.Rule = Empty | type_arguments;
      type_arguments.Rule = LT + type_argument_list + GT;
      type_argument_list.Rule = MakePlusRule(type_argument_list, COMMA, type_argument);
      type_argument.Rule = type;
      type_argument.Rule |= QMARK + EXTENDS + type;
      type_argument.Rule |= QMARK + SUPER_TOKEN + type;
      type_argument.Rule |= QMARK;

      type_parameters_opt.Rule = Empty | type_parameters;
      type_parameters.Rule = LT + type_parameter_list + GT;
      type_parameter_list.Rule = MakePlusRule(type_parameter_list, COMMA, type_parameter);
      type_parameter.Rule = type;
      type_parameter.Rule |= type + type_bound;
      type_bound_opt.Rule = Empty | type_bound;
      type_bound.Rule = EXTENDS + type_bound_list;
      type_bound_list.Rule = MakePlusRule(type_bound_list, AMP, interface_type);

      dims.Rule = MakeStarRule(dims, dim);
      dim.Rule = L_BKT + R_BKT;

      dim_exprs.Rule = MakePlusRule(dim_exprs, dim_expr);
      dim_expr.Rule = L_BKT + expression + R_BKT;

      method_declarator.Rule = identifier + L_PAR + formal_parameter_list_opt + R_PAR + dims;
      throws_opt.Rule = Empty | throws;
      throws.Rule = THROWS_TOKEN + exception_type_list;
      exception_type_list.Rule = MakePlusRule(exception_type_list, COMMA, templated_qualified_name);

      formal_parameter_list_opt.Rule = Empty | formal_parameter_list;
      formal_parameter_list.Rule  = formal_parameters + COMMA + last_formal_parameter;
      formal_parameter_list.Rule |=                             last_formal_parameter;
      formal_parameters.Rule = MakePlusRule(formal_parameters, COMMA, formal_parameter);
      formal_parameter.Rule  = modifiers_opt + primitive_type +           dims + identifier + dims;
      formal_parameter.Rule |= modifiers_opt + templated_qualified_name + dims + identifier + dims;
      last_formal_parameter.Rule  = formal_parameter;
      last_formal_parameter.Rule |= modifiers_opt + primitive_type           + dims + DOT_DOT_DOT + identifier + dims;
      last_formal_parameter.Rule |= modifiers_opt + templated_qualified_name + dims + DOT_DOT_DOT + identifier + dims;

      variable_declarators_rest.Rule = MakeStarRule(variable_declarators_rest, COMMA + variable_declarator);
      variable_declarators.Rule = MakePlusRule(variable_declarators, COMMA, variable_declarator);
      variable_declarator.Rule  = templated_qualified_name + dims;
      variable_declarator.Rule |= templated_qualified_name + dims + ASSIGN + variable_initializer;

      //variable_initializers.Rule = MakeStarRule(variable_initializers, COMMA, variable_initializer, TermListOptions.AllowTrailingDelimiter);
      variable_initializers.Rule = MakeListRule(variable_initializers, COMMA, variable_initializer, TermListOptions.StarList | TermListOptions.AllowTrailingDelimiter);
      variable_initializer.Rule = array_initializer;
      variable_initializer.Rule |= expression;

      array_initializer.Rule = L_BRC + variable_initializers + R_BRC;
      array_initializer.Rule |= L_BRC + COMMA + R_BRC;
      
      primitive_type.Rule  = BOOLEAN;
      primitive_type.Rule |= BYTE;
      primitive_type.Rule |= SHORT;
      primitive_type.Rule |= INT;
      primitive_type.Rule |= LONG;
      primitive_type.Rule |= CHAR;
      primitive_type.Rule |= FLOAT;
      primitive_type.Rule |= DOUBLE;

      literal.Rule = null_literal;
      literal.Rule |= number_literal;
      literal.Rule |= character_literal;
      literal.Rule |= string_literal;
      literal.Rule |= boolean_literal;

      boolean_literal.Rule = TRUE | FALSE;

      statement_expression.Rule = expression;

      block.Rule = L_BRC + block_statements + R_BRC;
      block_statements.Rule = MakeStarRule(block_statements, block_statement);
      block_statement.Rule = statement;
      block_statement.Rule |= explicit_constructor_invocation;
      block_statement.Rule |= modifiers + local_variable_declaration;
      block_statement.Rule |=             local_variable_declaration;
      block_statement.Rule |= modifiers + annotation_declaration;
      block_statement.Rule |=             annotation_declaration;
      block_statement.Rule |= modifiers + class_declaration;
      block_statement.Rule |=             class_declaration;
      block_statement.Rule |= modifiers + enum_declaration;
      block_statement.Rule |=             enum_declaration;
      block_statement.Rule |= modifiers + interface_declaration;
      block_statement.Rule |=             interface_declaration;

      local_variable_declaration.Rule = primitive_type + dim + dims + variable_declarators + SEMI;
      local_variable_declaration.Rule |= primitive_type +             variable_declarators + SEMI;
      local_variable_declaration.Rule |= templated_qualified_name + dim + dims + variable_declarators + SEMI;
      local_variable_declaration.Rule |= templated_qualified_name +              variable_declarators + SEMI;
      #endregion

      #region top level
      compilation_unit.Rule = Empty;
      compilation_unit.Rule |= package_declaration_w_modifiers + import_declarations + type_declarations;
      compilation_unit.Rule |= package_declaration +             import_declarations + type_declarations;
      compilation_unit.Rule |=                                   import_declarations + type_declarations;
      compilation_unit.Rule |= package_declaration_w_modifiers +                       type_declarations;
      compilation_unit.Rule |=             package_declaration +                       type_declarations;
      compilation_unit.Rule |=                                                         type_declarations;
      compilation_unit.Rule |= package_declaration_w_modifiers + import_declarations;
      compilation_unit.Rule |=             package_declaration + import_declarations;
      compilation_unit.Rule |=                                   import_declarations;
      compilation_unit.Rule |= package_declaration_w_modifiers;
      compilation_unit.Rule |=             package_declaration;

      package_declaration.Rule = PACKAGE + qualified_name + SEMI;
      package_declaration_w_modifiers.Rule = modifiers + PACKAGE + qualified_name + SEMI;

      import_declarations.Rule = MakePlusRule(import_declarations, import_declaration);
      import_declaration.Rule = normal_import_declaration | static_import_declaration;
      normal_import_declaration.Rule = IMPORT +          qualified_name + import_wildcard + SEMI;
      static_import_declaration.Rule = IMPORT + STATIC + qualified_name + import_wildcard + SEMI;
      import_wildcard.Rule = Empty | (DOT + STAR);

      type_declarations.Rule = MakePlusRule(type_declarations, type_declaration);
      type_declaration.Rule = type_declaration_w_modifiers | base_type_declaration | SEMI;

      type_declaration_w_modifiers.Rule = modifiers + base_type_declaration;
      base_type_declaration.Rule = annotation_declaration | class_declaration | enum_declaration | interface_declaration;
      #endregion

      #region type declarations
      annotation_declaration.Rule = AT + INTERFACE + identifier + L_BRC + annotation_type_body + R_BRC;
      class_declaration.Rule = CLASS_TOKEN + identifier + type_parameters_opt + super_opt + interfaces_opt + L_BRC + class_body + R_BRC;
      enum_declaration.Rule = ENUM + identifier + interfaces_opt + L_BRC + enum_body + R_BRC;
      interface_declaration.Rule  = INTERFACE + identifier + type_parameters_opt + EXTENDS + interface_type_list + L_BRC + interface_body + R_BRC;
      interface_declaration.Rule |= INTERFACE + identifier + type_parameters_opt +                                 L_BRC + interface_body + R_BRC;
      #endregion

      #region interface_declaration
      interface_body.Rule = MakeStarRule(interface_body, interface_member_declaration);
      interface_member_declaration.Rule = SEMI;
      interface_member_declaration.Rule |= modifiers_opt + constant_declaration;
      interface_member_declaration.Rule |= modifiers_opt + abstract_method_declaration;
      interface_member_declaration.Rule |= modifiers_opt + class_declaration;
      interface_member_declaration.Rule |= modifiers_opt + interface_declaration;

      // these type_parameters are not actually allowed but it saves a whole lot of work in resolving code.
      constant_declaration.Rule  = type_parameters_opt + primitive_type +           dims + variable_declarators + SEMI;
      constant_declaration.Rule |= type_parameters_opt + templated_qualified_name + dims + variable_declarators + SEMI;

      abstract_method_declaration.Rule  = type_parameters_opt + primitive_type +           dims + method_declarator + throws_opt + SEMI;
      abstract_method_declaration.Rule |= type_parameters_opt + templated_qualified_name + dims + method_declarator + throws_opt + SEMI;
      abstract_method_declaration.Rule |= type_parameters_opt + VOID +                     dims + method_declarator + throws_opt + SEMI;
      #endregion

      #region class_declaration
      class_body_opt.Rule = Empty | L_BRC + class_body + R_BRC;
      class_body.Rule = MakeStarRule(class_body, class_body_declaration);

      class_body_declaration.Rule = class_member_declaration;
      class_body_declaration.Rule |= instance_initializer;
      class_body_declaration.Rule |= static_initializer;
      class_body_declaration.Rule |= constructor_declaration;

      class_member_declaration.Rule = SEMI;
      class_member_declaration.Rule |= modifiers + field_declaration;
      class_member_declaration.Rule |=             field_declaration;
      class_member_declaration.Rule |= modifiers + method_declaration;
      class_member_declaration.Rule |=             method_declaration;
      class_member_declaration.Rule |= modifiers + annotation_declaration;
      class_member_declaration.Rule |=             annotation_declaration;
      class_member_declaration.Rule |= modifiers + class_declaration;
      class_member_declaration.Rule |=             class_declaration;
      class_member_declaration.Rule |= modifiers + enum_declaration;
      class_member_declaration.Rule |=             enum_declaration;
      class_member_declaration.Rule |= modifiers + interface_declaration;
      class_member_declaration.Rule |=             interface_declaration;

      // these type_parameters are not actually allowed but it saves a whole lot of work in resolving code.
      field_declaration.Rule  = type_parameters_opt + primitive_type +           dims + variable_declarators + SEMI;
      field_declaration.Rule |= type_parameters_opt + templated_qualified_name + dims + variable_declarators + SEMI;

      method_declaration.Rule  = type_parameters_opt + VOID +                            method_declarator + throws_opt + method_body;
      method_declaration.Rule |= type_parameters_opt + primitive_type +           dims + method_declarator + throws_opt + method_body;
      method_declaration.Rule |= type_parameters_opt + templated_qualified_name + dims + method_declarator + throws_opt + method_body;

      method_body.Rule = SEMI;
      method_body.Rule |= block;

      instance_initializer.Rule = block;
      static_initializer.Rule = STATIC + block;

      constructor_declaration.Rule  = modifiers + type_parameters_opt + identifier + L_PAR + formal_parameter_list_opt + R_PAR + throws_opt + constructor_body;
      constructor_declaration.Rule |=             type_parameters_opt + identifier + L_PAR + formal_parameter_list_opt + R_PAR + throws_opt + constructor_body;

      constructor_body.Rule = L_BRC + block_statements + R_BRC;

      explicit_constructor_invocation.Rule  = type_arguments_opt + THIS + arguments + SEMI;
      explicit_constructor_invocation.Rule |= type_arguments_opt + SUPER_TOKEN + arguments + SEMI;
      #endregion

      #region annotations
      //element_value_pairs.Rule = MakeStarRule(element_value_pairs, COMMA, element_value_pair, TermListOptions.AllowTrailingDelimiter);
      element_value_pairs.Rule = MakeListRule(element_value_pairs, COMMA, element_value_pair, TermListOptions.StarList | TermListOptions.AllowTrailingDelimiter);
      element_value_pair.Rule = identifier + ASSIGN + element_value;
      element_value_array_initializer.Rule = L_BRC + element_values + R_BRC;
      //element_values.Rule = MakeStarRule(element_values, COMMA, element_value, TermListOptions.AllowTrailingDelimiter);
      element_values.Rule = MakeListRule(element_values, COMMA, element_value, TermListOptions.StarList | TermListOptions.AllowTrailingDelimiter);
      element_value.Rule = annotation;
      element_value.Rule |= element_value_array_initializer;
      element_value.Rule |= expression;

      annotations.Rule = MakeStarRule(annotations, annotation);
      annotation.Rule  = AT + qualified_name + L_PAR + element_value_pair + R_PAR;
      annotation.Rule |= AT + qualified_name + L_PAR + element_value_pair + COMMA + element_value_pairs + R_PAR;
      annotation.Rule |= AT + qualified_name + L_PAR + element_values + R_PAR;
      annotation.Rule |= AT + qualified_name;


      annotation_type_body.Rule = MakeStarRule(annotation_type_body, annotation_type_element_declaration);
      annotation_type_element_declaration.Rule = modifiers + annotation_type_element_rest;
      annotation_type_element_declaration.Rule |=           annotation_type_element_rest;

      annotation_type_element_rest.Rule  = type + identifier + L_PAR + R_PAR + DEFAULT + element_value + SEMI;
      annotation_type_element_rest.Rule |= type + identifier + L_PAR + R_PAR + SEMI;
      annotation_type_element_rest.Rule |= type + variable_declarators + SEMI;
      annotation_type_element_rest.Rule |= class_declaration;
      annotation_type_element_rest.Rule |= enum_declaration;
      annotation_type_element_rest.Rule |= annotation_declaration;
      #endregion

      #region enum
      enum_body.Rule = enum_constants + enum_body_declarations_opt;
      enum_body.Rule |= COMMA + enum_body_declarations_opt;
      //enum_constants.Rule = MakeStarRule(enum_constants, COMMA, enum_constant, TermListOptions.AllowTrailingDelimiter);
      enum_constants.Rule = MakeListRule(enum_constants, COMMA, enum_constant, TermListOptions.StarList | TermListOptions.AllowTrailingDelimiter);
      enum_constant.Rule = annotations + identifier;
      enum_constant.Rule |= annotations + identifier + arguments;
      enum_constant.Rule |= annotations + identifier + arguments + L_BRC + class_body + R_BRC;
      enum_constant.Rule |= annotations + identifier             + L_BRC + class_body + R_BRC;

      arguments_opt.Rule = Empty | arguments;
      arguments.Rule = L_PAR + argument_list + R_PAR;
      argument_list.Rule = MakeStarRule(argument_list, COMMA, expression);

      enum_body_declarations_opt.Rule = Empty | enum_body_declarations;
      enum_body_declarations.Rule = SEMI + class_body;
      #endregion

      #region expressions
      super_suffix.Rule = arguments;
      super_suffix.Rule |= DOT + identifier + arguments_opt;

      explicit_generic_invocation_suffix.Rule = SUPER_TOKEN + super_suffix;
      explicit_generic_invocation_suffix.Rule |= identifier + arguments;

      array_creator_rest.Rule = dim + dims + array_initializer;
      array_creator_rest.Rule |= dim_exprs + dim + dims;
      array_creator_rest.Rule |= dim_exprs;

      class_creator_rest.Rule = arguments + class_body_opt;
      
      creator.Rule = type_arguments_opt + created_name + (array_creator_rest | class_creator_rest);
      created_name.Rule = templated_qualified_name;

      explicit_generic_invocation.Rule = type_arguments + PreferShiftHere() + explicit_generic_invocation_suffix;

      inner_creator.Rule = templated_qualified_name + class_creator_rest;

      identifier_suffix_opt.Rule = Empty | identifier_suffix;
      identifier_suffix.Rule = dim + dims + DOT + CLASS_TOKEN;
      identifier_suffix.Rule = arguments;
      identifier_suffix.Rule = DOT + CLASS_TOKEN;
      identifier_suffix.Rule |= DOT + explicit_generic_invocation;
      identifier_suffix.Rule |= DOT + THIS;
      identifier_suffix.Rule |= DOT + type_arguments_opt + SUPER_TOKEN + arguments;
      identifier_suffix.Rule |= DOT + NEW + type_arguments_opt + inner_creator;

      selector.Rule = DOT + explicit_generic_invocation;
      selector.Rule |= DOT + THIS;
      selector.Rule |= DOT + type_arguments_opt + SUPER_TOKEN + arguments;
      selector.Rule |= DOT + NEW + type_arguments_opt + inner_creator;
      selector.Rule |= L_BKT + expression + R_BKT;

      primary_expression.Rule = primary_expression_no_new;
      primary_expression.Rule |= NEW + creator;

      expression_in_parens.Rule = L_PAR + expression + R_PAR;
      expression_in_parens.Rule |= L_PAR + templated_qualified_name + R_PAR;

      cast_expression.Rule = L_PAR + templated_qualified_name + R_PAR + expression;
      cast_expression.Rule |= L_PAR + templated_qualified_name + dim + dims + R_PAR + expression;
      cast_expression.Rule |= L_PAR + primitive_type + R_PAR + expression;
      cast_expression.Rule |= L_PAR + primitive_type + dim + dims + R_PAR + expression;

      primary_expression_no_new.Rule = expression_in_parens;
      primary_expression_no_new.Rule |= cast_expression;
      primary_expression_no_new.Rule |= THIS;
      primary_expression_no_new.Rule |= literal;
      primary_expression_no_new.Rule |= templated_qualified_name + dim + dims + identifier_suffix;
      primary_expression_no_new.Rule |= templated_qualified_name + dim + dims;
      primary_expression_no_new.Rule |= templated_qualified_name + identifier_suffix;
      primary_expression_no_new.Rule |= templated_qualified_name;
      primary_expression_no_new.Rule |= primitive_type + dims + DOT + CLASS_TOKEN;
      primary_expression_no_new.Rule |= VOID + DOT + CLASS_TOKEN;
      primary_expression_no_new.Rule |= array_access;
      primary_expression_no_new.Rule |= field_access;
      primary_expression_no_new.Rule |= method_invocation;

      method_invocation.Rule  = expression +                                                                                  arguments;
      method_invocation.Rule |= expression +                                          DOT + type_arguments_opt + identifier + arguments;
      method_invocation.Rule |=                    type_arguments_opt + SUPER_TOKEN + DOT + type_arguments_opt + identifier + arguments;
      method_invocation.Rule |= expression + DOT + type_arguments_opt + SUPER_TOKEN + DOT + type_arguments_opt + identifier + arguments;
      method_invocation.Rule |=                    type_arguments_opt + THIS +        DOT + type_arguments_opt + identifier + arguments;
      method_invocation.Rule |= expression + DOT + type_arguments_opt + THIS +        DOT + type_arguments_opt + identifier + arguments;

      field_access.Rule  = expression +                                          DOT + type_arguments_opt + identifier;
      field_access.Rule |=                    type_arguments_opt + SUPER_TOKEN + DOT + type_arguments_opt + identifier;
      field_access.Rule |= expression + DOT + type_arguments_opt + SUPER_TOKEN + DOT + type_arguments_opt + identifier;
      field_access.Rule |=                    type_arguments_opt + THIS +        DOT + type_arguments_opt + identifier;
      field_access.Rule |= expression + DOT + type_arguments_opt + THIS +        DOT + type_arguments_opt + identifier;

      assignment_operator.Rule = ASSIGN | PLUS_ASSIGN | MINUS_ASSIGN | STAR_ASSIGN | SLASH_ASSIGN | AMP_ASSIGN | 
                    BAR_ASSIGN | CARET_ASSIGN | PERCENT_ASSIGN | SHL_ASSIGN  | SHR_ASSIGN   | USHR_ASSIGN ;


      infix_operator.Rule = BAR_BAR | AMP_AMP | BAR | AMP | CARET | EQ | NEQ | LT | GT | LTEQ | GTEQ | SHR | SHL | USHR |
                            PLUS | MINUS | STAR | SLASH | PERCENT | INSTANCEOF;

      prefix_operator.Rule = PLUS_PLUS | MINUS_MINUS | EMARK | TILDE | PLUS | MINUS;
      postfix_operator.Rule = PLUS_PLUS | MINUS_MINUS;

      array_access.Rule = templated_qualified_name + dim_expr;
      array_access.Rule |= primary_expression_no_new + dim_expr;
      array_access.Rule |= array_access + dim_expr;

      left_hand_side.Rule = templated_qualified_name;
      left_hand_side.Rule |= expression_in_parens;
      left_hand_side.Rule |= field_access;
      left_hand_side.Rule |= array_access;

      unary_expression.Rule = prefix_operator + expression;
      unary_expression.Rule |= expression + selector + postfix_operator;
      unary_expression.Rule |= expression + selector;
      unary_expression.Rule |= expression +            postfix_operator;

      binary_expression.Rule = expression + infix_operator + expression;
      trinary_expression.Rule = expression + QMARK + expression + COLON + expression;

      assignment_expression.Rule = left_hand_side + assignment_operator + expression;
      
      expression.Rule = primary_expression;
      expression.Rule |= assignment_expression;
      expression.Rule |= unary_expression;
      expression.Rule |= binary_expression;
      expression.Rule |= trinary_expression;
      #endregion

      #region statements
      statement.Rule = block;
      statement.Rule |= ASSERT + expression + SEMI;
      statement.Rule |= ASSERT + expression + COLON + expression + SEMI;
      statement.Rule |= IF + L_PAR + expression + R_PAR + statement;
      statement.Rule |= IF + L_PAR + expression + R_PAR + statement + PreferShiftHere() + ELSE + statement;
      statement.Rule |= FOR + L_PAR + for_control + R_PAR + statement;
      statement.Rule |= WHILE + L_PAR + expression + R_PAR + statement;
      statement.Rule |= DO + statement + WHILE + L_PAR + expression + R_PAR + SEMI;
      statement.Rule |= TRY + block + catches + FINALLY_TOKEN + block;
      statement.Rule |= TRY + block + catches;
      statement.Rule |= TRY + block +           FINALLY_TOKEN + block;
      statement.Rule |= SWITCH + L_PAR + expression + R_PAR + L_BRC + switch_block_statement_groups + R_BRC;
      statement.Rule |= SYNCHRONIZED + L_PAR + expression + R_PAR + block;
      statement.Rule |= RETURN + expression + SEMI;
      statement.Rule |= RETURN + SEMI;
      statement.Rule |= THROW + expression + SEMI;
      statement.Rule |= BREAK + identifier + SEMI;
      statement.Rule |= BREAK + SEMI;
      statement.Rule |= CONTINUE + identifier + SEMI;
      statement.Rule |= CONTINUE + SEMI;
      statement.Rule |= SEMI;
      statement.Rule |= statement_expression + SEMI;
      statement.Rule |= identifier + COLON + statement;

      for_control.Rule = for_var_control;
      for_control.Rule |= for_init + SEMI + expression + SEMI + for_update;
      for_control.Rule |= for_init + SEMI + expression + SEMI;
      for_control.Rule |= for_init + SEMI +              SEMI + for_update;
      for_control.Rule |= for_init + SEMI +              SEMI;

      for_var_control.Rule  = modifiers + type + identifier + COLON + expression;
      for_var_control.Rule |=             type + identifier + COLON + expression;

      for_init.Rule = for_update;
      for_init.Rule |= modifiers + type + variable_declarators;
      for_init.Rule |=         type + variable_declarators;

      for_update.Rule = MakePlusRule(for_update, COMMA, statement_expression);

      catches.Rule = MakePlusRule(catches, catch_clause);
      catch_clause.Rule = CATCH + L_PAR + formal_parameter + R_PAR + block;

      switch_block_statement_groups.Rule = MakeStarRule(switch_block_statement_groups, switch_block_statement_group);
      switch_block_statement_group.Rule = switch_label + block_statements;
      switch_label.Rule = CASE + expression + COLON;
      switch_label.Rule |= DEFAULT + COLON;
      #endregion

      #region operator precedence
      RegisterOperators(1, Associativity.Right, ASSIGN, PLUS_ASSIGN, MINUS_ASSIGN, STAR_ASSIGN, SLASH_ASSIGN, AMP_ASSIGN, BAR_ASSIGN, CARET_ASSIGN, PERCENT_ASSIGN, SHL_ASSIGN, SHR_ASSIGN, USHR_ASSIGN);
      RegisterOperators(2, Associativity.Right, QMARK);
      RegisterOperators(3, Associativity.Left, BAR_BAR);
      RegisterOperators(4, Associativity.Left, AMP_AMP);
      RegisterOperators(5, Associativity.Left, BAR);
      RegisterOperators(6, Associativity.Left, CARET);
      RegisterOperators(7, Associativity.Left, AMP);
      RegisterOperators(8, Associativity.Left, EQ, NEQ);
      RegisterOperators(9, Associativity.Left, INSTANCEOF, GT, GTEQ, LT, LTEQ);
      RegisterOperators(10, Associativity.Left, SHL, SHR, USHR);
      RegisterOperators(11, Associativity.Left, PLUS, MINUS);
      RegisterOperators(12, Associativity.Left, STAR, SLASH, PERCENT);
      RegisterOperators(13, Associativity.Right, PLUS_PLUS, MINUS_MINUS, TILDE, EMARK, NEW);
      RegisterOperators(14, Associativity.Left, DOT);
      RegisterOperators(15, Associativity.Neutral, R_PAR, R_BKT);
      #endregion
#endregion

      Root = compilation_unit;

      mSkipTokensInPreview.UnionWith(new Terminal[] { identifier_raw, DOT_RAW, COMMA, L_BKT_RAW, R_BKT, QMARK });
      MarkTransient(
        #region Transients
        type_parameters_opt,
        super_opt,
        interfaces_opt,
        type_arguments_opt,
        type_bound_opt,
        type_declaration,
        import_declaration,
        modifier,
        modifiers_opt,
        enum_body_declarations_opt,
        expression,
        L_BKT,
        L_PAR,
        LT
        #endregion
        );
    }
  }
}
