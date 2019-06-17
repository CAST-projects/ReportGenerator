<?xml version="1.0" encoding="utf-8"?>
<!-- 
£ $HeadURL: svn://gargantua/DEV/branches/7.0.11/_build/nant/vsprojconvert.xsl $
£ $Id: vsprojconvert.xsl 47230 2010-11-24 17:03:37Z YLE $
£ Creator : YLE@CAST                                                          £
£ Authors : YLE@CAST                                                          £
£ What it is : an XSLT stylesheet to transform visual studio projects into    £
£ NAnt build files.                                                           £
-->


<xsl:stylesheet version="1.0"
            xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
            xmlns:nm="http://schemas.microsoft.com/developer/msbuild/2003"
            xmlns="http://nant.sf.net/release/0.90/nant.xsd"
            exclude-result-prefixes="nm" >

    <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

    <!-- Parameters of the stylesheet -->
    <xsl:param name="project_full_path"/>
    <xsl:param name="project_name"/>
    <xsl:param name="project_dir"/>
    <xsl:param name="bin_dir"/>

    <xsl:template name="target_type">
        <xsl:value-of select="translate(//nm:Project/nm:PropertyGroup/nm:OutputType,'LE','le')"/>
    </xsl:template>

   
    <!-- Processing -->
    <xsl:template match="nm:Project">
        <xsl:comment> ************************************************************************* </xsl:comment>
        <xsl:comment> Generated file, changes made to this file will be lost upon regeneration. </xsl:comment>
        
        <xsl:comment> NAnt -buildfile:&lt;path of this file&gt; [options] [&lt;target&gt;] ...  </xsl:comment>
        <xsl:comment> ************************************************************************* </xsl:comment>
        <project default="all">
            <xsl:attribute name="basedir">
                <xsl:value-of select="$project_dir"/>
            </xsl:attribute>
            <xsl:attribute name="name">
               <xsl:value-of select="$project_name"/>
            </xsl:attribute>

            <!-- global properties of the output build file -->
            <property name="project_full_path" value="{$project_full_path}"/>
            <property name="clr_version_major" value="${{version::get-major(framework::get-clr-version(framework::get-target-framework()))}}"/>
            <property name="clr_version_minor" value="${{version::get-minor(framework::get-clr-version(framework::get-target-framework()))}}"/>

            <xsl:call-template name="firstPropertyGroup">
                <xsl:with-param name="node" select="nm:PropertyGroup[position() = 1]"/>
            </xsl:call-template>


            <xsl:for-each select="nm:PropertyGroup[position() != 1]">
                <xsl:if test="@Condition">
                    <xsl:call-template name="configuration_target">
                        <xsl:with-param name="node" select="."/>
                    </xsl:call-template>
                </xsl:if>
            </xsl:for-each>

            <!-- This the default target of the output file, it calls the targets corresponding to each configuration -->
            <xsl:comment> Default target, calls the targets corresponding to each configuration. </xsl:comment>
            <target name="all">
                <echo message="Building all in ${{project::get-base-directory()}}"/>
                <xsl:for-each select="nm:PropertyGroup[position() != 1]">
                    <xsl:if test="@Condition">
                        <xsl:call-template name="all_target_entry">
                            <xsl:with-param name="node" select="."/>
                        </xsl:call-template>
                    </xsl:if>
                </xsl:for-each>
            </target>

            <xsl:comment> 'clean' target, cleans the output of all configurations. </xsl:comment>
            <target name="clean">
                <xsl:for-each select="nm:PropertyGroup[position() != 1]">
                    <xsl:if test="@Condition">
                        <xsl:call-template name="clean_entry">
                            <xsl:with-param name="node" select="."/>
                        </xsl:call-template>
                    </xsl:if>
                </xsl:for-each>
            </target>

            <xsl:for-each select="nm:PropertyGroup[position() != 1]">
                <xsl:if test="@Condition">
                    <xsl:variable name="config_name"><xsl:call-template name="get_configuration_name"/></xsl:variable>
                    <xsl:if test="$config_name = 'debug' or $config_name = 'release'">

                        <xsl:comment> '<xsl:value-of select="$config_name"/>' configuration clean up. </xsl:comment>
                        <target name="clean_{$config_name}" depends="init_{$config_name}">
                            <delete file="${{target_path}}" verbose="true" if="${{file::exists(target_path)}}"/>
                        </target>
                    </xsl:if>
                </xsl:if>
            </xsl:for-each>

            <xsl:comment> ****************************************************** </xsl:comment>
            <xsl:comment> 'compile' runs the compilation for each configuration. </xsl:comment>
            <xsl:comment> ****************************************************** </xsl:comment>
            <target name="compile">
                <mkdir dir="${{target_dir}}"/>
                <csc output="${{target_path}}"
                     optimize="${{optimize}}"
                     debug="${{debug}}"
                     define="${{defines}};NET_${{clr_version_major}}${{clr_version_minor}}"
                     platform="${{platform}}"
                     noconfig="true">
                    <xsl:attribute name="target"><xsl:call-template name="target_type"/></xsl:attribute>
                    <sources>
                        <xsl:apply-templates select="nm:ItemGroup/nm:Compile"/>
                    </sources>
                    <references>
                        <lib>
                            <include name="${{framework::get-assembly-directory(framework::get-target-framework())}}"/>
                            <include name="{$bin_dir}/${{configuration_name}}"/>
                        </lib>
                        <xsl:apply-templates select="nm:ItemGroup/nm:Reference"/>
                        <!-- System.Core is mandatory when framework version is higher than 4.0 
                             @todo: add only if not already present. -->
                        <include name="System.Core.dll" if="${{version::get-major(framework::get-clr-version(framework::get-target-framework())) >= 4}}"/>
                        <xsl:apply-templates select="nm:ItemGroup/nm:ProjectReference"/>
                    </references>
                </csc>
            </target>
            <xsl:comment> ****************************************************** </xsl:comment>
            <xsl:comment> Post build step. </xsl:comment>
            <xsl:comment> For the moment it is not the post build step found in the </xsl:comment>
            <xsl:comment> VS project, it is just a copy to the target direcory. </xsl:comment>
            <xsl:comment> ****************************************************** </xsl:comment>
            <target name="post_build">
                <copy file="${{target_path}}" todir="{$bin_dir}/${{configuration_name}}"/>
            </target>
        </project>
    </xsl:template>

    <!-- Straight assembly reference -->
    <xsl:template match="/nm:Project/nm:ItemGroup/nm:Reference">
        <include name="{@Include}.dll"/>
    </xsl:template>

    <!-- Reference done through project -->
    <xsl:template match="/nm:Project/nm:ItemGroup/nm:ProjectReference">
        <xsl:variable name="project_name" select="nm:Name"/>
        <xsl:variable name="dependency_name_temp">
            <xsl:choose>
                <xsl:when test="string-length($project_name) > 5 and substring($project_name, string-length($project_name)-4) = '.2005'">
                    <xsl:value-of select="substring($project_name, 1, string-length($project_name)-5)"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="$project_name"/>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="dependency_name" select="concat($dependency_name_temp,'-${clr_version_major}.${clr_version_minor}.dll')"/>
        <include name="{$dependency_name}"/>
    </xsl:template>


    <xsl:template name="get_configuration_name">
        <xsl:choose>
            <xsl:when test="contains(@Condition,'Debug|AnyCPU')">debug</xsl:when>
            <xsl:when test="contains(@Condition,'Release|AnyCPU')">release</xsl:when>
        </xsl:choose>
    </xsl:template>

    <xsl:template match="/nm:Project/nm:ItemGroup/nm:Compile">
        <include name="{@Include}"/>
    </xsl:template>

    <xsl:template name="firstPropertyGroup">
        <xsl:param name="node"/>
        <xsl:variable name="extension">
            <xsl:choose>
                <xsl:when test="$node/nm:OutputType = 'Library'">dll</xsl:when>
                <xsl:when test="$node/nm:OutputType = 'Exe'">exe</xsl:when>
                <xsl:when test="$node/nm:OutputType = 'WinExe'">exe</xsl:when>
            </xsl:choose>
        </xsl:variable>
        <xsl:variable name="name" select="concat($node/nm:RootNamespace,'-${clr_version_major}.${clr_version_minor}')"/>
        <property name="assembly_file_name">
            <xsl:attribute name="value">
                <xsl:value-of select="concat(concat($node/nm:AssemblyName,'.'), $extension)"/>
            </xsl:attribute>
        </property>
        <property name="output_file_name" value="{$name}.{$extension}"/>
        <property name="output_name" value="{$name}"/>
        <property name="output_extension">
            <xsl:attribute name="value">
                <xsl:value-of select="$extension"/>
            </xsl:attribute>
        </property>
    </xsl:template>

    
    <xsl:template name="configuration_target">
        <xsl:param name="node"/>
        <xsl:variable name="config_name"><xsl:call-template name="get_configuration_name"/></xsl:variable>
        <xsl:variable name="debug" select="$config_name = 'debug'"/>
        <xsl:if test="$config_name = 'debug' or $config_name = 'release'">
            <xsl:comment> Target corresponding to '<xsl:value-of select="$config_name"/>' configuration. </xsl:comment>
            <target name="{$config_name}" depends="init_{$config_name},clean_{$config_name}">
                <call target="compile"/>
                <call target="post_build"/>
            </target>
            <xsl:comment> '<xsl:value-of select="$config_name"/>' configuration initialization. </xsl:comment>
            <target name="init_{$config_name}">
                <property name="configuration_name" value="{$config_name}"/>
                <property name="debug" value="{string(boolean($debug))}"/>
                <property name="optimize" value="{$node/nm:Optimize}"/>
                <property name="defines">
                    <xsl:attribute name="value">
                        <xsl:choose>
                            <xsl:when test="contains($node/nm:DefineConstants, ';NET_20')">
                                <xsl:value-of select="concat( substring-before($node/nm:DefineConstants, ';NET_20')
                                                            , substring-after($node/nm:DefineConstants, ';NET_20'))"/>
                            </xsl:when>
                            <xsl:otherwise>
                                <xsl:value-of select="$node/nm:DefineConstants"/>
                            </xsl:otherwise>
                        </xsl:choose> 
                    </xsl:attribute>
                </property>
                <property name="platform" value="{$node/nm:PlatformTarget}"/>
                <!-- @todo: replace the hardcoded bin/${config_name} by the output path in the vsproj.
                £   This is not yet done cause we have to remove any trailing '\' or '/'
                £   before we append the file name
                -->
                <property name="target_dir" value="bin/{$config_name}"/>
                <property name="target_path" value="bin/{$config_name}/${{output_file_name}}"/>
            </target>
        </xsl:if>
    </xsl:template>

    <xsl:template name="all_target_entry">
        <xsl:param name="node"/>
        <xsl:variable name="config_name"><xsl:call-template name="get_configuration_name"/></xsl:variable>
        <xsl:if test="$config_name = 'debug' or $config_name = 'release'">
            <call target="{$config_name}"/>
        </xsl:if>
    </xsl:template>

    <xsl:template name="clean_entry">
        <xsl:param name="node"/>
        <xsl:variable name="config_name"><xsl:call-template name="get_configuration_name"/></xsl:variable>
        <xsl:if test="$config_name = 'debug' or $config_name = 'release'">
            <call target="clean_{$config_name}"/>
        </xsl:if>
    </xsl:template>

</xsl:stylesheet>

