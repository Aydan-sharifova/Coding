import { useEffect, useRef } from "react";
import type { Material, Mesh } from "three";

export function AuthMotionScene() {
  const canvasRef = useRef<HTMLCanvasElement>(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    let cancelled = false;
    let disposeScene = () => undefined;

    void import("three").then((THREE) => {
      if (cancelled) return;

      const scene = new THREE.Scene();
      const camera = new THREE.PerspectiveCamera(38, 1, 0.1, 100);
      camera.position.set(0, 0, 8.2);

      const renderer = new THREE.WebGLRenderer({
        canvas,
        alpha: true,
        antialias: true,
        powerPreference: "high-performance",
      });
      renderer.setPixelRatio(Math.min(window.devicePixelRatio, 1.5));
      renderer.outputColorSpace = THREE.SRGBColorSpace;

      const group = new THREE.Group();
      group.rotation.set(-0.16, 0.38, 0.08);
      scene.add(group);

      const coreGeometry = new THREE.IcosahedronGeometry(1.42, 3);
      const coreMaterial = new THREE.MeshPhysicalMaterial({
        color: 0x7567ff,
        emissive: 0x24195e,
        emissiveIntensity: 0.8,
        roughness: 0.24,
        metalness: 0.35,
        transmission: 0.08,
        transparent: true,
        opacity: 0.92,
      });
      const core = new THREE.Mesh(coreGeometry, coreMaterial);
      group.add(core);

      const wireMaterial = new THREE.MeshBasicMaterial({
        color: 0xb9d5ff,
        wireframe: true,
        transparent: true,
        opacity: 0.2,
      });
      const wire = new THREE.Mesh(
        new THREE.IcosahedronGeometry(1.68, 2),
        wireMaterial,
      );
      group.add(wire);

      const ringMaterial = new THREE.MeshBasicMaterial({
        color: 0x79e6d3,
        transparent: true,
        opacity: 0.48,
      });
      const ring = new THREE.Mesh(
        new THREE.TorusGeometry(2.2, 0.018, 12, 180),
        ringMaterial,
      );
      ring.rotation.set(1.1, 0.3, 0.28);
      group.add(ring);

      const secondRing = new THREE.Mesh(
        new THREE.TorusGeometry(2.55, 0.012, 10, 180),
        new THREE.MeshBasicMaterial({
          color: 0x9aa6ff,
          transparent: true,
          opacity: 0.28,
        }),
      );
      secondRing.rotation.set(0.35, 1.05, -0.5);
      group.add(secondRing);

      const nodeGeometry = new THREE.SphereGeometry(0.09, 18, 18);
      const nodeMaterial = new THREE.MeshBasicMaterial({ color: 0xb5fff1 });
      const nodes: Mesh[] = [];
      for (let index = 0; index < 7; index += 1) {
        const angle = (index / 7) * Math.PI * 2;
        const node = new THREE.Mesh(nodeGeometry, nodeMaterial);
        node.position.set(
          Math.cos(angle) * 2.2,
          Math.sin(angle) * 0.68,
          Math.sin(angle) * 1.85,
        );
        nodes.push(node);
        group.add(node);
      }

      const starPositions = new Float32Array(150 * 3);
      for (let index = 0; index < 150; index += 1) {
        const offset = index * 3;
        starPositions[offset] = (Math.random() - 0.5) * 12;
        starPositions[offset + 1] = (Math.random() - 0.5) * 10;
        starPositions[offset + 2] = (Math.random() - 0.5) * 7 - 1;
      }
      const starsGeometry = new THREE.BufferGeometry();
      starsGeometry.setAttribute(
        "position",
        new THREE.BufferAttribute(starPositions, 3),
      );
      const stars = new THREE.Points(
        starsGeometry,
        new THREE.PointsMaterial({
          color: 0xcbd3ff,
          size: 0.025,
          transparent: true,
          opacity: 0.58,
        }),
      );
      scene.add(stars);

      scene.add(new THREE.AmbientLight(0x8d9aff, 1.8));
      const keyLight = new THREE.PointLight(0x71e4d0, 18, 16);
      keyLight.position.set(3.5, 3, 4);
      scene.add(keyLight);
      const fillLight = new THREE.PointLight(0x7b61ff, 22, 18);
      fillLight.position.set(-4, -2, 3);
      scene.add(fillLight);

      let pointerX = 0;
      let pointerY = 0;
      let frame = 0;
      const reduceMotion = window.matchMedia(
        "(prefers-reduced-motion: reduce)",
      ).matches;

      const resize = () => {
        const width = Math.max(canvas.clientWidth, 1);
        const height = Math.max(canvas.clientHeight, 1);
        renderer.setSize(width, height, false);
        camera.aspect = width / height;
        camera.updateProjectionMatrix();
        renderer.render(scene, camera);
      };
      const resizeObserver = new ResizeObserver(resize);
      resizeObserver.observe(canvas);
      resize();

      const handlePointer = (event: PointerEvent) => {
        const bounds = canvas.getBoundingClientRect();
        pointerX = ((event.clientX - bounds.left) / bounds.width - 0.5) * 0.5;
        pointerY = ((event.clientY - bounds.top) / bounds.height - 0.5) * 0.35;
      };
      canvas.addEventListener("pointermove", handlePointer, { passive: true });

      const render = (time: number) => {
        const seconds = time * 0.001;
        group.rotation.y += (0.38 + pointerX - group.rotation.y) * 0.035;
        group.rotation.x += (-0.16 + pointerY - group.rotation.x) * 0.035;
        core.rotation.y = seconds * 0.16;
        core.rotation.x = seconds * 0.08;
        wire.rotation.y = -seconds * 0.1;
        ring.rotation.z = seconds * 0.08;
        secondRing.rotation.z = -seconds * 0.06;
        stars.rotation.y = seconds * 0.008;
        nodes.forEach((node, index) => {
          node.scale.setScalar(0.82 + Math.sin(seconds * 2 + index) * 0.16);
        });
        renderer.render(scene, camera);
        frame = window.requestAnimationFrame(render);
      };

      if (reduceMotion) {
        renderer.render(scene, camera);
      } else {
        frame = window.requestAnimationFrame(render);
      }

      disposeScene = () => {
        window.cancelAnimationFrame(frame);
        resizeObserver.disconnect();
        canvas.removeEventListener("pointermove", handlePointer);
        coreGeometry.dispose();
        coreMaterial.dispose();
        wire.geometry.dispose();
        wireMaterial.dispose();
        ring.geometry.dispose();
        ringMaterial.dispose();
        secondRing.geometry.dispose();
        (secondRing.material as Material).dispose();
        nodeGeometry.dispose();
        nodeMaterial.dispose();
        starsGeometry.dispose();
        (stars.material as Material).dispose();
        renderer.dispose();
      };
    });

    return () => {
      cancelled = true;
      disposeScene();
    };
  }, []);

  return (
    <div className="auth-motion-scene" aria-hidden="true">
      <canvas ref={canvasRef} />
      <span className="auth-motion-orbit auth-motion-orbit-one" />
      <span className="auth-motion-orbit auth-motion-orbit-two" />
    </div>
  );
}
