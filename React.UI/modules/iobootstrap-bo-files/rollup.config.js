import { nodeResolve } from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';
import typescript from '@rollup/plugin-typescript';

import pkg from './package.json';

export default {
  input: 'src/index.ts',
  output: [
    {
      dir: './lib',
      entryFileNames: 'cjs/[name].js',
      format: 'cjs',
      sourcemap: 'inline'
    },
    {
      dir: './lib',
      entryFileNames: 'esm/[name].js',
      format: 'es',
      sourcemap: 'inline'
    },
  ],
  external: [...Object.keys(pkg.peerDependencies || {})],
  plugins: [
    nodeResolve({
      extensions: ['.ts', '.tsx', '.json', '.js', '.jsx'] 
    }),
    typescript({
      tsconfig: './tsconfig.json'
    }),
    commonjs()
  ],
};
