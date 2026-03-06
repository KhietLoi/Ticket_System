/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	
    config.height = 350;
    config.uiColor = '#d3d3d3';
    config.extraPlugins = 'embedbase';
    config.extraPlugins = 'embed';
    config.embed_provider = '//ckeditor.iframe.ly/api/oembed?url={url}&callback={callback}';
};
