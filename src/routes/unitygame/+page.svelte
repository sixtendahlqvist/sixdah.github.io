<script>
  import { onMount } from 'svelte';

  let unityInstance = null;
  let container, canvas, loadingBar, progressBarFull, fullscreenButton, warningBanner;

  onMount(() => {
    const buildUrl = "Build";
    const loaderUrl = `${buildUrl}/Documents.loader.js`;

    const config = {
      dataUrl: `${buildUrl}/Documents.data.br`,
      frameworkUrl: `${buildUrl}/Documents.framework.js.br`,
      codeUrl: `${buildUrl}/Documents.wasm.br`,
      streamingAssetsUrl: "StreamingAssets",
      companyName: "Sixten Dahlqvist inc. AB",
      productName: "StardewCraft",
      productVersion: "1.0",
      showBanner: unityShowBanner,
    };

    if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
      container.className = "unity-mobile";
      canvas.className = "unity-mobile";
      unityShowBanner('WebGL builds are not supported on mobile devices.');
    } else {
      canvas.style.width = "960px";
      canvas.style.height = "600px";
    }

    loadingBar.style.display = "block";

    const script = document.createElement("script");
    script.src = loaderUrl;
    script.onload = () => {
      createUnityInstance(canvas, config, (progress) => {
        progressBarFull.style.width = 100 * progress + "%";
      }).then(instance => {
        unityInstance = instance;
        loadingBar.style.display = "none";
        fullscreenButton.onclick = () => {
          unityInstance.SetFullscreen(1);
        };
      }).catch((message) => {
        alert(message);
      });
    };
    document.body.appendChild(script);
  });

  function unityShowBanner(msg, type) {
    function updateBannerVisibility() {
      warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
    }
    const div = document.createElement('div');
    div.innerHTML = msg;
    warningBanner.appendChild(div);
    if (type === 'error') div.style = 'background: red; padding: 10px;';
    else if (type === 'warning') {
      div.style = 'background: yellow; padding: 10px;';
      setTimeout(() => {
        warningBanner.removeChild(div);
        updateBannerVisibility();
      }, 5000);
    }
    updateBannerVisibility();
  }
</script>

<div bind:this={container} id="unity-container" class="unity-desktop">
  <canvas bind:this={canvas} id="unity-canvas" width="960" height="600"></canvas>
  <div bind:this={loadingBar} id="unity-loading-bar">
    <div id="unity-logo"></div>
    <div id="unity-progress-bar-empty">
      <div bind:this={progressBarFull} id="unity-progress-bar-full"></div>
    </div>
  </div>
  <div bind:this={warningBanner} id="unity-warning"></div>
  <div id="unity-footer">
    <div id="unity-webgl-logo"></div>
    <button bind:this={fullscreenButton} id="unity-fullscreen-button">Fullscreen</button>
    <div id="unity-build-title">StardewCraft</div>
  </div>
</div>
