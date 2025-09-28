// vite.config.ts
import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig(({ mode }) => {
  // .env.* dosyalarını mode'a göre yükle (isteğe bağlı kullanırsınız)
  // const env = loadEnv(mode, process.cwd(), '')

  const now = new Date()
  const dateString = `${now.getFullYear()}.${now.getMonth() + 1}.${now.getDate()}-${now.getHours()}.${now.getMinutes()}.${now.getSeconds()}`

  return {
    plugins: [react()],
    server: {
        port: 3000,   // localhost:3000
        open: '/backoffice/'  // otomatik olarak /backoffice yolunda tarayıcı aç
    },
    base: '/backoffice/',
    // CRA ile aynı klasörü istiyorsanız:
    build: {
      outDir: 'build',
      rollupOptions: {
        output: {
          // JS entry ve chunk dosyaları:
          entryFileNames: `static/js/[name].${dateString}.js`,
          chunkFileNames: `static/js/[name].chunk.${dateString}.js`,
          // CSS için özel pattern, diğer asset'lerde hash kalsın:
          assetFileNames: (assetInfo) => {
            const name = assetInfo.name ?? ''
            if (name.endsWith('.css')) {
              return `static/css/[name].${dateString}.css`
            }
            // Diğer tüm asset'ler: hash'li kalsın (cache-busting için iyi)
            return `assets/[name].[hash][extname]`
          }
        }
      }
    },
  }
})