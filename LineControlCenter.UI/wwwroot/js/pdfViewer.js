// pdfViewer.js  — ES module
// Imported lazily by PdfPageCanvas.razor via JS.InvokeAsync<IJSObjectReference>
// so it is NEVER served from a browser global-script cache.
//
// pdfjsLib is assigned to window.pdfjsLib by the inline <script type="module"> in
// App.razor (the PDF.js CDN loader block); we read it via globalThis.

const _docCache = new Map();   // url → Promise<PDFDocumentProxy>

function _getDoc(url) {
    if (_docCache.has(url)) return _docCache.get(url);
    const lib = globalThis.pdfjsLib;
    if (!lib) throw new Error('[pdfViewer] pdfjsLib is not loaded yet.');
    const p = lib.getDocument(url).promise;
    _docCache.set(url, p);
    return p;
}

function _normalise(deg) {
    const n = Math.round(Number(deg) / 90) * 90;
    return ((n % 360) + 360) % 360;
}

export async function renderPage(canvasId, url, pageNumber, extraRotation) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) { console.warn('[pdfViewer] canvas not found:', canvasId); return; }

    const ctx  = canvas.getContext('2d');
    const doc  = await _getDoc(url);
    const page = await doc.getPage(pageNumber);

    // Use rotation: 0 — ignore ALL metadata and all caller overrides.
    // The PDFs are authored so their content is visually correct at 0°.
    const rotation = 0;

    const container = canvas.parentElement;
    const cW = (container && container.clientWidth)  || window.innerWidth;
    const cH = (container && container.clientHeight) || window.innerHeight;

    const base     = page.getViewport({ scale: 1, rotation: 0 });
    const scale    = Math.min(cW / base.width, cH / base.height);
    const viewport = page.getViewport({ scale, rotation: 0 });

    const dpr = window.devicePixelRatio || 1;
    canvas.width  = Math.floor(viewport.width  * dpr);
    canvas.height = Math.floor(viewport.height * dpr);
    canvas.style.width  = viewport.width  + 'px';
    canvas.style.height = viewport.height + 'px';

    console.debug('[pdfViewer] render', { url, pageNumber, 'page.rotate': page.rotate, rotation });

    await page.render({
        canvasContext: ctx,
        viewport,
        transform: dpr !== 1 ? [dpr, 0, 0, dpr, 0, 0] : null
    }).promise;
}

export function destroyDoc(url) {
    const p = _docCache.get(url);
    if (!p) return;
    _docCache.delete(url);
    p.then(doc => { try { doc.destroy(); } catch { /**/ } }).catch(() => { /**/ });
}
