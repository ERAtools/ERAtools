<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<div align="center">
			<h2>Environmental Resource Analysis</h2>
			<h4>Resources At Risk </h4>
		</div>
		<b>
			<i>Analysis Shape Type: <xsl:value-of select="/RESULTS/@analysis_shape"/>
			</i>
		</b>
		<br/>
		<b>
			<i>Analysis Timestamp: <xsl:value-of select="/RESULTS/@datetime"/>
			</i>
		</b>
		<br/>
		<b>
			<i>Shape Name:<xsl:value-of select="/RESULTS/@shape_name"/>
			</i>
		</b>
		<br/>
		<b>
			<i>Boundary Area: <xsl:value-of select='format-number(/RESULTS/@analysis_acreage, "####.##")'/> (acres)</i>
		</b>
		<br/>
		<b>
			<i>Buffer Area: <xsl:value-of select='format-number(/RESULTS/@buffer_acreage, "####.##")'/> (acres)</i>
		</b>
		<br/>
		<b>
			<i>Total Area: <xsl:value-of select='format-number(/RESULTS/@analysis_acreage + /RESULTS/@buffer_acreage, "####.##")'/> (acres)</i>
		</b>
		<br/>
		<xsl:for-each select="RESULTS/ISSUE">
			<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0" style="border: solid 2px #000;">
				<tr>
					<td height="35" bgcolor="#D7D5D5" valign="center">
						<font face="verdana,arial,helvetica" size="3">
							<b>&#160;<xsl:value-of select="@name"/>
							</b>
						</font>
					</td>
				</tr>
				<tr>
					<td bgcolor="#aaaaaa" colspan="3">
						<xsl:for-each select="LAYER">
							<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
								<tr>
									<td bgcolor="#ccddcc" width="1%"/>
									<td bgcolor="#ccddcc" valign="center">
										<font face="verdana,arial,helvetica" size="2">
											<b>&#160;<xsl:value-of select="@name"/></b>
										</font>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
											<tr>
												<td bgcolor="#92B692" width="3%"/>
												<xsl:for-each select="FIELDS/FIELD">
													<td bgcolor="#92B692">
														<font face="verdana,arial,helvetica" size="1">
															<b>
																<xsl:value-of select="@alias"/>
															</b>
														</font>
													</td>
												</xsl:for-each>
												<!--FIELDS/FIELD-->
											</tr>
											<xsl:for-each select="RECORD">
												<xsl:sort select="DISTANCE/@order" data-type="number" order="ascending"/>
												<tr>
													<xsl:choose>
														<xsl:when test="position()mod 2=0">
															<xsl:attribute name="style">background-color:#CCCCCC;</xsl:attribute>
														</xsl:when>
														<xsl:otherwise>
															<xsl:attribute name="style">background-color:#F2FFF2;</xsl:attribute>
														</xsl:otherwise>
													</xsl:choose>
													<td width="3%">&#160;</td>
													<xsl:for-each select="./VALUES/*">
														<td>
															<xsl:choose>
																<xsl:when test="@link = 'true'">
																	<xsl:choose>
																		<xsl:when test="@useforurl">
																			<A target="_blank">
																				<xsl:attribute name="href"><xsl:value-of select="./@useforurl"/></xsl:attribute>
																				<font face="verdana,arial,helvetica" size="1">
																					<xsl:value-of select="."/>
																				</font>
																			</A>
																		</xsl:when>
																		<xsl:otherwise>
																			<A target="_blank">
																				<xsl:attribute name="href"><xsl:value-of select="."/></xsl:attribute>
																				<font face="verdana,arial,helvetica" size="1">
																					<xsl:value-of select="."/>
																				</font>
																			</A>
																		</xsl:otherwise>
																	</xsl:choose>
																</xsl:when>
																<xsl:otherwise>
																	<font face="verdana,arial,helvetica" size="1">
																		<xsl:value-of select="."/>
																	</font>
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
		
		<xsl:for-each select="RESULTS/ERROR">
			<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
				<tr>
					<td>
						<br/>
					</td>
				</tr>
				<tr>
					<td bgcolor="#aaaaaa">
						<font face="verdana,arial,helvetica" size="2">
							<b>
								<xsl:value-of select="@name"/>
							</b>
						</font>
					</td>
				</tr>
				<tr>
					<td>
						<xsl:for-each select="LAYER">
							<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
								<tr>
									<td bgcolor="#cccccc" width="1"/>
									<td bgcolor="#cccccc">
										<font face="verdana,arial,helvetica" size="2">
											<xsl:value-of select="@msg"/>
											<b>
												<xsl:value-of select="@name"/>
											</b>
										</font>
									</td>
								</tr>
								<tr>
									<td colspan="2">
										<xsl:for-each select="FIELDS">
											<xsl:for-each select="FIELD">
												<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
													<tr>
														<td bgcolor="#92B692" width="30"/>
														<td bgcolor="#92B692">
															<font face="verdana,arial,helvetica" size="1">
																<xsl:value-of select="@msg"/>
																<b>
																	<xsl:value-of select="@name"/>
																</b>
															</font>
														</td>
													</tr>
												</table>
											</xsl:for-each>
											<!--FIELD-->
										</xsl:for-each>
										<!--FIELDS-->
									</td>
								</tr>
							</table>
						</xsl:for-each>
						<!--LAYER-->
					</td>
				</tr>
			</table>
		</xsl:for-each>
		<!--RESULTS/ERROR-->
	</xsl:template>
	
	<!-- PROCESS TABLES -->
	<xsl:template name="process_tables">
		<xsl:param name="tables" />
		<xsl:param name="colspan" />
		<xsl:param name="level" />
		<tr>
			<td></td>
			<td>
				<xsl:attribute name="colspan">
					<xsl:value-of select="$colspan"/>
				</xsl:attribute>
				<xsl:for-each select="$tables">
					<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0" style="border: solid 2px #000;">
						<tr>
							<td bgcolor="#ccddcc" width="1%"/>
							<td bgcolor="#ccddcc" valign="center">
								<font face="verdana,arial,helvetica" size="2">
									<b>&#160;<xsl:value-of select="@name"/></b>
								</font>
							</td>
						</tr>
						<tr>
							<td colspan="2">
								<table width="100%" border="0" CELLPADDING="0" CELLSPACING="0">
									<tr>
										<td style="background-color: #92B692;" width="3%"></td>
										<xsl:for-each select="FIELDS/FIELD">
											<td style="background-color: #92B692;">
												<font face="verdana,arial,helvetica" size="1">
													<b>
														<xsl:value-of select="@alias"/>
													</b>
												</font>
											</td>
										</xsl:for-each>
										<!--FIELDS/FIELD-->
									</tr>
									<xsl:for-each select="RECORD">
										<xsl:sort select="DISTANCE/@order" data-type="number" order="ascending"/>
										<tr>
											<xsl:choose>
												<xsl:when test="position()mod 2=0">
													<xsl:attribute name="style">background-color:#CCCCCC;</xsl:attribute>
												</xsl:when>
												<xsl:otherwise>
													<xsl:attribute name="style">background-color:#F2FFF2;</xsl:attribute>
												</xsl:otherwise>
											</xsl:choose>
											<td width="3%"></td>
											<xsl:for-each select="./VALUES/*">
												<td>
													<xsl:choose>
														<xsl:when test="@link = 'true'">
															<xsl:choose>
																<xsl:when test="@useforurl">
																	<A target="_blank">
																		<xsl:attribute name="href"><xsl:value-of select="./@useforurl"/></xsl:attribute>
																		<font face="verdana,arial,helvetica" size="1">
																			<xsl:value-of select="."/>
																		</font>
																	</A>
																</xsl:when>
																<xsl:otherwise>
																	<A target="_blank">
																		<xsl:attribute name="href"><xsl:value-of select="."/></xsl:attribute>
																		<font face="verdana,arial,helvetica" size="1">
																			<xsl:value-of select="."/>
																		</font>
																	</A>
																</xsl:otherwise>
															</xsl:choose>
														</xsl:when>
														<xsl:otherwise>
															<font face="verdana,arial,helvetica" size="1">
																<xsl:value-of select="."/>
															</font>
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
