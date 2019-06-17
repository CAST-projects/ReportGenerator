<?xml version="1.0" encoding="utf-8"?>
<!-- 
£ $HeadURL: svn://gargantua/DEV/branches/7.0.11/_build/nant/vsprojdepends.xsl $
£ $Id: vsprojdepends.xsl 45145 2010-09-22 16:21:23Z YLE $
£ Creator : YLE@CAST                                                          £
£ Authors : YLE@CAST                                                          £
£ What it is : stylesheet used to generate a dependency file from a NAnt      £
£ build file.                                                                 £
-->

<xsl:stylesheet version="1.0"
            xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
            xmlns:na="http://nant.sf.net/release/0.90/nant.xsd"
            exclude-result-prefixes="na" >

    <xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

    <xsl:template match="na:project">
        <xsl:comment> Generated file </xsl:comment>
        <xsl:comment> Dependencies of project <xsl:value-of select="@name"/> </xsl:comment>
        <project_dependencies>
            <project name="{@name}">
                <xsl:apply-templates select="na:property[@name='output_file_name' and @value]"/>
                <xsl:apply-templates select="na:target/na:csc/na:references/na:include"/>
            </project>
        </project_dependencies>            
    </xsl:template>

    <xsl:template match="na:property[@name='output_file_name' and @value]">
        <xsl:attribute name="output_name">
            <xsl:value-of select="@value"/>
        </xsl:attribute>
    </xsl:template>

    <xsl:template match="//na:target/na:csc/na:references/na:include">
        <dependency name="{@name}"/>
    </xsl:template>

</xsl:stylesheet>

    
