<!--htm-->
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="html" indent="yes" />
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <title>Environmental Resource Analysis</title>
        <style type="text/css">
          .IssueName
          {
          padding: 8px 5px 2px 10px;
          background-color: #BBBBDD;
          font-size: medium;
          font-weight: bold;
          }
          .IssueDesc
          {
          padding: 2px 5px 8px 10px;
          background-color: #BBBBDD;
          font-size: x-small;
          font-weight: bold;
          }
          .DarkGray
          {
          background-color: #AAAAAA;
          }
          .TableName
          {
          background-color: #FFDD77;
          padding-left: 10px;
          font-size: x-small;
          font-weight: bold;
          }
          .HeaderRow
          {
          background-color: #FFDD77;
          font-size: xx-small;
          font-weight: bold;
          }
          .DataRow1
          {
          background-color: #DDEEEE;
          font-size: xx-small;
          }
          .DataRow2
          {
          background-color: #FFFFFF;
          font-size: xx-small;
          }
        </style>
      </head>
      <body>
        <div align="center">
          <img src="http://www.doh.state.fl.us/environment/images/template/divheader.gif" alt="Division of Environmental Health" style="float: left;" />
          <img alt="Department of Health" style="float: right;">
            <xsl:attribute name="src">
              <xsl:value-of select="/RESULTS/@system_path"/>/dohheader.gif
            </xsl:attribute>
          </img>
          <h2>Environmental Resource Analysis</h2>
          <h4>
            <xsl:value-of select="/RESULTS/@title"/>
          </h4>
        </div>
        <table width="100%">
          <tr>
            <td style="font-style: italic; font-weight: bold;">
              Analysis Shape Type: <xsl:value-of select="/RESULTS/@analysis_shape"/><br/>
              Analysis Timestamp: <xsl:value-of select="/RESULTS/@datetime"/><br/>
              Shape Name: <xsl:value-of select="/RESULTS/@shape_name"/><br/>
              Boundary Area: <xsl:value-of select='format-number(/RESULTS/@analysis_acreage, "###0.##")'/> acres<br/>
              Buffer Area: <xsl:value-of select='format-number(/RESULTS/@buffer_acreage, "###0.##")'/> acres<br/>
              Total Area: <xsl:value-of select='format-number(/RESULTS/@analysis_acreage + /RESULTS/@buffer_acreage, "###0.##")'/> acres
            </td>
            <td style="border-color: Black; border-width: 2px; border-style: solid; font-size: medium; font-weight: bold;">
              <span style="font-size: large;">HVA Index Key</span>
              <hr style="color: Black;" />
              <span style="font-size: xx-large; float: left;">
                <xsl:text disable-output-escaping="yes">&amp;darr;</xsl:text>
              </span>
              0 = Least Vulnerable<br /><br />
              4 = Most Vulnerable
            </td>
          </tr>
        </table>
        <xsl:for-each select="RESULTS/ISSUE">
          <table width="100%" border="0" CELLPADDING="0" CELLSPACING="0" style="border: solid 2px #000; font-family: Verdana, Arial, Helvetica;">
            <tr>
              <td class="IssueName">
                <xsl:value-of select="@name" />
              </td>
            </tr>
            <xsl:if test="@description">
              <tr>
                <td class="IssueDesc">
                  <xsl:value-of select="@description" />
                </td>
              </tr>
            </xsl:if>
            <tr>
              <td class="DarkGray">
                <xsl:for-each select="LAYER">
                  <table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
                    <tr>
                      <td class="TableName">
                        <xsl:value-of select="@name"/>
                      </td>
                    </tr>
                    <tr>
                      <td>
                        <table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
                          <xsl:if test="not(FIELDS/@showheaders = 'false')">
                            <tr class="HeaderRow">
                              <td width="3%"/>
                              <xsl:for-each select="FIELDS/FIELD">
                                <td>
                                  <xsl:value-of select="@alias"/>
                                </td>
                              </xsl:for-each>
                              <!--FIELDS/FIELD-->
                            </tr>
                          </xsl:if>
                          <xsl:for-each select="RECORD">
                            <!--<xsl:sort select="VALUES/DISTANCE/@order" data-type="number" order="ascending"/>-->
                            <tr>
                              <xsl:choose>
                                <xsl:when test="position() mod 2 = 0">
                                  <xsl:attribute name="class">DataRow2</xsl:attribute>
                                </xsl:when>
                                <xsl:otherwise>
                                  <xsl:attribute name="class">DataRow1</xsl:attribute>
                                </xsl:otherwise>
                              </xsl:choose>
                              <td />
                              <xsl:for-each select="./VALUES/*">
                                <td>
                                  <xsl:choose>
                                    <xsl:when test="@link = 'true'">
                                      <xsl:choose>
                                        <xsl:when test="@useforurl">
                                          <A target="_blank">
                                            <xsl:attribute name="href">
                                              <xsl:value-of select="./@useforurl"/>
                                            </xsl:attribute>
                                            <xsl:value-of select="."/>
                                          </A>
                                        </xsl:when>
                                        <xsl:otherwise>
                                          <A target="_blank">
                                            <xsl:attribute name="href">
                                              <xsl:value-of select="."/>
                                            </xsl:attribute>
                                            <xsl:value-of select="."/>
                                          </A>
                                        </xsl:otherwise>
                                      </xsl:choose>
                                    </xsl:when>
                                    <xsl:otherwise>
                                      <xsl:value-of select="."/>
                                    </xsl:otherwise>
                                  </xsl:choose>
                                </td>
                              </xsl:for-each>
                              <!--VALUES-->
                            </tr>
                            <xsl:if test="count(./TABLE) > 0">
                              <xsl:call-template name="process_tables">
                                <xsl:with-param name="tables" select="./TABLE" />
                                <xsl:with-param name="colspan" select="count(./VALUES/*)" />
                                <xsl:with-param name="level" select="2" />
                              </xsl:call-template>
                            </xsl:if>
                          </xsl:for-each>
                          <!--RECORDS-->
                        </table>
                      </td>
                    </tr>
                  </table>
                </xsl:for-each>
                <!--LAYER-->
              </td>
            </tr>
          </table>
        </xsl:for-each>
        <!--RESULTS/ISSUE-->

        <!--RESULTS/ERROR-->
        <ul style="color: Red">
          <xsl:for-each select="RESULTS/ERROR">
            <li>
              Error evaluating <xsl:value-of select="@nodetype"/> node <xsl:value-of select="@name"/>: <xsl:value-of select="@message"/><br/>
              Occurred in <xsl:value-of select="@function"/>
            </li>
          </xsl:for-each>
        </ul>
        <!--RESULTS/ERROR-->
      </body>
    </html>
  </xsl:template>

  <!-- PROCESS TABLES -->
  <xsl:template name="process_tables">
    <xsl:param name="tables" />
    <xsl:param name="colspan" />
    <xsl:param name="level" />
    <tr>
      <td />
      <td>
        <xsl:attribute name="colspan">
          <xsl:value-of select="$colspan"/>
        </xsl:attribute>
        <xsl:for-each select="$tables">
          <table width="100%" border="0" CELLPADDING="0" CELLSPACING="0" style="border: solid 2px #000;">
            <tr>
              <td class="TableName">
                <xsl:value-of select="@name"/>
              </td>
            </tr>
            <tr>
              <td>
                <table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
                  <xsl:if test="not(FIELDS/@showheaders = 'false')">
                    <tr class="HeaderRow">
                      <td width="3%"></td>
                      <xsl:for-each select="FIELDS/FIELD">
                        <td>
                          <xsl:value-of select="@alias"/>
                        </td>
                      </xsl:for-each>
                      <!--FIELDS/FIELD-->
                    </tr>
                  </xsl:if>
                  <xsl:for-each select="RECORD">
                    <!--<xsl:sort select="DISTANCE/@order" data-type="number" order="ascending"/>-->
                    <tr>
                      <xsl:choose>
                        <xsl:when test="position()mod 2=0">
                          <xsl:attribute name="class">DataRow2</xsl:attribute>
                        </xsl:when>
                        <xsl:otherwise>
                          <xsl:attribute name="class">DataRow1</xsl:attribute>
                        </xsl:otherwise>
                      </xsl:choose>
                      <td />
                      <xsl:for-each select="./VALUES/*">
                        <td>
                          <xsl:choose>
                            <xsl:when test="@link = 'true'">
                              <xsl:choose>
                                <xsl:when test="@useforurl">
                                  <A target="_blank">
                                    <xsl:attribute name="href">
                                      <xsl:value-of select="./@useforurl"/>
                                    </xsl:attribute>
                                    <xsl:value-of select="."/>
                                  </A>
                                </xsl:when>
                                <xsl:otherwise>
                                  <A target="_blank">
                                    <xsl:attribute name="href">
                                      <xsl:value-of select="."/>
                                    </xsl:attribute>
                                    <xsl:value-of select="."/>
                                  </A>
                                </xsl:otherwise>
                              </xsl:choose>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="."/>
                            </xsl:otherwise>
                          </xsl:choose>
                        </td>
                      </xsl:for-each>
                      <!--VALUES-->
                    </tr>
                    <xsl:if test="count(./TABLE) > 0">
                      <xsl:call-template name="process_tables">
                        <xsl:with-param name="tables" select="./TABLE" />
                        <xsl:with-param name="colspan" select="count(./VALUES/*)" />
                        <xsl:with-param name="level" select="$level + 1" />
                      </xsl:call-template>
                    </xsl:if>
                  </xsl:for-each>
                  <!--RECORDS-->
                </table>
              </td>
            </tr>
          </table>
        </xsl:for-each>
        <!-- TABLES -->
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
